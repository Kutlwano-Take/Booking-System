namespace PracticeBookingSystem.Models;

public class Practitioner
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string? Specialty { get; set; }
    public int PracticeId { get; set; }
}
