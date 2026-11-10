using PracticeBookingSystem.Data;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services;
using PracticeBookingSystem.Services.Interfaces;
using Xunit;

namespace PracticeBookingSystem.Tests;

public class AppointmentServiceTests
{
    private readonly DemoAppointmentStore _store = new();
    private readonly AppointmentService _sut;

    public AppointmentServiceTests()
    {
        _sut = new AppointmentService(_store, new NoOpNotificationService());
    }

    private CreateAppointmentRequest Request(DateTime startsAt, int practitionerId = 1, int locationId = 1,
        int patientId = 1, int duration = 60, int buffer = 30) => new()
    {
        PatientId = patientId,
        PractitionerId = practitionerId,
        LocationId = locationId,
        StartsAt = startsAt,
        DurationMinutes = duration,
        BufferMinutes = buffer
    };

    private static DateTime TestDate => DateTime.Today.AddDays(3).AddHours(8);

    [Fact]
    public void Book_ValidFutureAppointment_CreatesConfirmedAppointment()
    {
        var request = Request(TestDate);

        var result = _sut.Book(request);

        Assert.True(result.Succeeded);
        Assert.NotNull(result.Appointment);
        Assert.Equal(AppointmentStatus.Confirmed, result.Appointment!.Status);
        Assert.Equal(TestDate.AddMinutes(60), result.Appointment.EndsAt);
        Assert.Contains(result.Appointment, _store.Appointments);
    }

    [Fact]
    public void Book_UnknownPatient_ReturnsError()
    {
        var result = _sut.Book(Request(TestDate, patientId: 999));

        Assert.False(result.Succeeded);
        Assert.Equal("Patient not found.", result.Error);
    }

    [Fact]
    public void Book_UnknownPractitioner_ReturnsError()
    {
        var result = _sut.Book(Request(TestDate, practitionerId: 999));

        Assert.False(result.Succeeded);
        Assert.Equal("Practitioner not found.", result.Error);
    }

    [Fact]
    public void Book_UnknownLocation_ReturnsError()
    {
        var result = _sut.Book(Request(TestDate, locationId: 999));

        Assert.False(result.Succeeded);
        Assert.Equal("Practice location not found.", result.Error);
    }

    [Fact]
    public void IsAvailable_OverlappingAppointmentAtDifferentLocation_IsUnavailable()
    {
        var first = _sut.Book(Request(TestDate, locationId: 1, buffer: 30));
        Assert.True(first.Succeeded);

        // Different location, same practitioner; overlaps with first booking's buffer.
        var available = _sut.IsAvailable(1, TestDate.AddMinutes(75),
            TestDate.AddMinutes(135), 30);

        Assert.False(available);
    }

    [Fact]
    public void IsAvailable_AtBufferBoundary_IsAvailable()
    {
        var first = _sut.Book(Request(TestDate, buffer: 30));
        Assert.True(first.Succeeded);

        var available = _sut.IsAvailable(1, TestDate.AddMinutes(90),
            TestDate.AddMinutes(150), 30);

        Assert.True(available);
    }

    [Fact]
    public void Cancel_ValidReason_CancelsAndReleasesSlot()
    {
        var booked = _sut.Book(Request(TestDate));
        Assert.True(booked.Succeeded);

        var cancelled = _sut.Cancel(booked.Appointment!.Id, "Patient requested cancellation");

        Assert.True(cancelled.Succeeded);
        Assert.Equal(AppointmentStatus.Cancelled, cancelled.Appointment!.Status);
        Assert.True(_sut.IsAvailable(1, TestDate, TestDate.AddMinutes(60), 30));
    }

    [Fact]
    public void Cancel_BlankReason_ReturnsErrorAndLeavesAppointmentConfirmed()
    {
        var booked = _sut.Book(Request(TestDate));
        Assert.True(booked.Succeeded);

        var cancelled = _sut.Cancel(booked.Appointment!.Id, " ");

        Assert.False(cancelled.Succeeded);
        Assert.Equal(AppointmentStatus.Confirmed, booked.Appointment.Status);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ReturnsError()
    {
        var booked = _sut.Book(Request(TestDate));
        Assert.True(booked.Succeeded);
        _sut.Cancel(booked.Appointment!.Id, "Change of plans");

        var secondCancel = _sut.Cancel(booked.Appointment.Id, "Another reason");

        Assert.False(secondCancel.Succeeded);
        Assert.Equal("Appointment is already cancelled.", secondCancel.Error);
    }

    [Fact]
    public void Book_PastAppointment_IsRejected()
    {
        var result = _sut.Book(Request(DateTime.Now.AddHours(-1)));

        Assert.False(result.Succeeded);
        Assert.Equal("Appointment must be in the future.", result.Error);
    }

    private sealed class NoOpNotificationService : INotificationService
    {
        public Task SendConfirmationAsync(Appointment appointment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendReminderAsync(Appointment appointment, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task SendCancellationAsync(Appointment appointment, string reason, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}
