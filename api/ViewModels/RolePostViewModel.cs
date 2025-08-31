using System.ComponentModel.DataAnnotations;

namespace api.ViewModels;

public class RolePostViewModel
{
    [Required]
    public string RoleName { get; set; }
}
