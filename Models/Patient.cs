using System.ComponentModel.DataAnnotations;

namespace PracticeBookingSystem.Models;

public class Patient
{
    public int Id { get; set; }
    [Required, StringLength(120)] public string FullName { get; set; } = string.Empty;
    [StringLength(30)] public string? IdentityNumber { get; set; }
    [StringLength(30)] public string? FileNumber { get; set; }
    [StringLength(120)] public string? MedicalAid { get; set; }
    [EmailAddress, StringLength(160)] public string Email { get; set; } = string.Empty;
    [Phone, StringLength(30)] public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    [StringLength(500)] public string? Notes { get; set; }
}
