using System.ComponentModel.DataAnnotations;

namespace PracticeBookingSystem.Models;

public class LoginViewModel
{
    //[Required, Display(Name = "Email or username")]
    //public string Username { get; set; } = "";

    //[Required, DataType(DataType.Password)]
    //public string Password { get; set; } = "";

    //[Display(Name = "Remember me")]
    //public bool RememberMe { get; set; }
    [Required(ErrorMessage = "Enter your email or username")]
    [Display(Name = "Email or username")]
    public string Username { get; set; } = "";

    [Required(ErrorMessage = "Enter your password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
