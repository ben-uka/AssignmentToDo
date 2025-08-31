using System.ComponentModel.DataAnnotations;

namespace api.ViewModels;

public class RegisterUserViewModel
{
    [Required]
    public string FirstName { get; set; } = "";

    [Required]
    public string LastName { get; set; } = "";

    [Required]
    [EmailAddress]
    public string Email { get; set; } = "";

    public string Password { get; set; } = "";
}
