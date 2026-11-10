using PracticeBookingSystem.Models;
using PracticeBookingSystem.Services.Interfaces;
namespace PracticeBookingSystem.Services;
public class NotificationService(IPracticeDataService practiceData, IEmailSender email) : INotificationService
{
    public async Task SendConfirmationAsync(Appointment a, CancellationToken ct = default)
    {
        var patient = practiceData.GetPatients().First(p => p.Id == a.PatientId);

        var practitioner = practiceData.GetPractitioners().First(p => p.Id == a.PractitionerId);

        var location = practiceData.GetLocations().First(l => l.Id == a.LocationId);

        await email.SendAsync(patient.Email, "Appointment confirmed",
            $"Hello {patient.FullName}, your appointment with {practitioner.FullName} is confirmed for {a.StartsAt:dddd, dd MMM yyyy 'at' HH:mm} at {location.Name}.", ct);
    }
    public async Task SendReminderAsync(Appointment a, CancellationToken ct = default)
    {
        var patient = practiceData.GetPatients().First(p => p.Id == a.PatientId);

        var practitioner = practiceData.GetPractitioners().First(p => p.Id == a.PractitionerId);

        var location = practiceData.GetLocations().First(l => l.Id == a.LocationId);

        await email.SendAsync(patient.Email, "Appointment reminder",
            $"Hello {patient.FullName}, reminder of your appointment today at {a.StartsAt:HH:mm} with {practitioner.FullName}, at {location.Name}.", ct);
    }
    public async Task SendCancellationAsync(Appointment a, string reason, CancellationToken ct = default)
    {
        var patient = practiceData.GetPatients().First(p => p.Id == a.PatientId);

        await email.SendAsync(patient.Email, "Appointment cancelled",
            $"Hello {patient.FullName}, your appointment scheduled for {a.StartsAt:dd MMM yyyy HH:mm} has been cancelled. Reason: {reason}", ct);
    }
}
