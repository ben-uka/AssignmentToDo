using Microsoft.AspNetCore.Identity;

namespace api.Data;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public IList<UserToDoList> UserToDoLists { get; set; } = new List<UserToDoList>();
}
