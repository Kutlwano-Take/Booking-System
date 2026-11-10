using Microsoft.EntityFrameworkCore;
using PracticeBookingSystem.Data;
using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services.Interfaces;

namespace PracticeBookingSystem.Services;

public sealed class EfAppointmentService(PracticeBookingDbContext db, INotificationService notifications) : IAppointmentService
{
    public IReadOnlyList<Appointment> GetAll() => db.Appointments.AsNoTracking().OrderBy(x => x.StartsAt).ToList();
    public Appointment? GetById(int id) => db.Appointments.FirstOrDefault(x => x.Id == id);

    public bool IsAvailable(int practitionerId, DateTime startsAt, DateTime endsAt, int bufferMinutes, int? excludingId = null)
    {
        if (endsAt <= startsAt || bufferMinutes < 0) return false;
        var blockedUntil = endsAt.AddMinutes(bufferMinutes);
        return !db.Appointments.Any(existing => existing.PractitionerId == practitionerId &&
            existing.Status != AppointmentStatus.Cancelled && existing.Id != excludingId &&
            startsAt < existing.EndsAt.AddMinutes(existing.BufferMinutes) && existing.StartsAt < blockedUntil);
    }

    public AppointmentResult Book(CreateAppointmentRequest request)
    {
        if (!db.Patients.Any(x => x.Id == request.PatientId)) 
            return new(false, null, "Patient not found.");

        if (!db.Practitioners.Any(x => x.Id == request.PractitionerId)) 
            return new(false, null, "Practitioner not found.");

        if (!db.Locations.Any(x => x.Id == request.LocationId)) 
            return new(false, null, "Practice location not found.");

        if (request.StartsAt <= DateTime.Now || request.DurationMinutes <= 0 || request.BufferMinutes < 0) 
            return new(false, null, "Choose a future date, valid duration, and non-negative buffer.");

        using var tx = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
        if (!IsAvailable(request.PractitionerId, request.StartsAt, request.StartsAt.AddMinutes(request.DurationMinutes), request.BufferMinutes))
            return new(false, null, "Practitioner is already booked during this time (including buffer).");
        var appointment = new Appointment { PatientId = request.PatientId, PractitionerId = request.PractitionerId, LocationId = request.LocationId,
            StartsAt = request.StartsAt, EndsAt = request.StartsAt.AddMinutes(request.DurationMinutes), BufferMinutes = request.BufferMinutes, Notes = request.Notes, Status = AppointmentStatus.Confirmed };
        db.Appointments.Add(appointment); 
        db.SaveChanges(); 
        tx.Commit();
        return new(true, appointment, null);
    }

    public AppointmentResult Cancel(int appointmentId, string reason)
    {
        var appointment = GetById(appointmentId);

        if (appointment is null) 
            return new(false, null, "Appointment not found.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            return new(false, appointment, "Appointment is already cancelled.");

        if (string.IsNullOrWhiteSpace(reason)) 
            return new(false, appointment, "A cancellation reason is required.");
        appointment.Status = AppointmentStatus.Cancelled; appointment.CancelledAt = DateTime.Now; db.SaveChanges();
        return new(true, appointment, null);
    }
}
