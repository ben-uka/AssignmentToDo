namespace api.ViewModels;

public class ToDoListViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public DateTime TimeCreated { get; set; }
}
