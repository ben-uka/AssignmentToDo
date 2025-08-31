using System.ComponentModel.DataAnnotations;

namespace api.ViewModels;

public class ToDoListPostViewModel
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = "";

    [Required]
    [MinLength(1)]
    public string Content { get; set; } = "";
}
