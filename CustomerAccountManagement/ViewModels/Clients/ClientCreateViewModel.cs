using System.ComponentModel.DataAnnotations;

namespace CustomerAccountManagement.ViewModels.Clients;

public class ClientCreateViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [MaxLength(200)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(320)]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;
}
