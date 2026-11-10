using PracticeBookingSystem.Models;
namespace PracticeBookingSystem.Services.Interfaces;
public interface INotificationService
{
    Task SendConfirmationAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task SendReminderAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task SendCancellationAsync(Appointment appointment, string reason, CancellationToken cancellationToken = default);
}
