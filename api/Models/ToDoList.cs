namespace api.Data;

public class ToDoList
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public DateTime TimeCreated { get; set; } = DateTime.UtcNow;
    public IList<UserToDoList> UserToDoLists { get; set; } = new List<UserToDoList>();
}
