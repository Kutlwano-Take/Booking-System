namespace PracticeBookingSystem.Models;
public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int PractitionerId { get; set; }
    public int LocationId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int BufferMinutes { get; set; } = 30;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CancelledAt { get; set; }
    public bool ConfirmationEmailSent { get; set; }
    public bool ReminderEmailSent { get; set; }
}
