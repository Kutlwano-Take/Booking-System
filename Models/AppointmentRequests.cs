using System.ComponentModel.DataAnnotations;
namespace PracticeBookingSystem.Models;
public class CreateAppointmentRequest
{
    [Range(1, int.MaxValue)] public int PatientId { get; set; }
    [Range(1, int.MaxValue)] public int PractitionerId { get; set; }
    [Range(1, int.MaxValue)] public int LocationId { get; set; }
    [Required] public DateTime StartsAt { get; set; }
    [Range(5, 480)] public int DurationMinutes { get; set; } = 60;
    [Range(0, 180)] public int BufferMinutes { get; set; } = 30;
    public string? Notes { get; set; }
}
public class CancelAppointmentRequest
{
    [Required] public string Reason { get; set; } = "";
}
public record AppointmentResult(bool Succeeded, Appointment? Appointment, string? Error);
