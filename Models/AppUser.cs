namespace PracticeBookingSystem.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Username { get; set; } = "";
        public string DisplayName { get; set; } = "";   // "Thabiso Mokoena"
        public string JobTitle { get; set; } = "";      // "Practice administrator"
        public string? PasswordHash { get; set; }       // null for Google-only accounts
        public string? GoogleSubject { get; set; }      // filled in when Google sign-in is added
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
    }
}
