namespace api.Data;

public class UserToDoList
{
    public string UserId { get; set; }
    public int ToDoListId { get; set; }
    public User User { get; set; }
    public ToDoList ToDoList { get; set; }
}
