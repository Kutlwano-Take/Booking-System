using PracticeBookingSystem.Models;
namespace PracticeBookingSystem.Services.Interfaces;
public interface IAppointmentService
{
    IReadOnlyList<Appointment> GetAll();
    Appointment? GetById(int id);
    AppointmentResult Book(CreateAppointmentRequest request);
    AppointmentResult Cancel(int appointmentId, string reason);
    bool IsAvailable(int practitionerId, DateTime startsAt, DateTime endsAt, int bufferMinutes, int? excludingId = null);
}
