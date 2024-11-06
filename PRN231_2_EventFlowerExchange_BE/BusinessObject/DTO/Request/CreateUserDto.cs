using System.ComponentModel.DataAnnotations;

public class CreateUserDTO
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public string Role { get; set; } 
}
