using System.ComponentModel.DataAnnotations;

// Models/RegisterViewModel.cs
public class RegisterViewModel
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string PhoneNumber { get; set; }
}