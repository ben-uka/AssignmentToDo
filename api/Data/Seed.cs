using Microsoft.AspNetCore.Identity;

namespace api.Data;

public class Seed
{
    public static async Task SeedData(
        DataContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager
    )
    {
        // Skapa roller om de inte finns
        if (!await roleManager.RoleExistsAsync("User"))
            await roleManager.CreateAsync(new IdentityRole("User"));
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole("Admin"));

        if (!userManager.Users.Any())
        {
            var users = new List<User>
            {
                new()
                {
                    FirstName = "Michael",
                    LastName = "Gustavsson",
                    UserName = "michael@gmail.com",
                    Email = "michael@gmail.com",
                },
                new()
                {
                    FirstName = "Benny",
                    LastName = "Andersson",
                    UserName = "benny@gmail.com",
                    Email = "benny@gmail.com",
                },
                new()
                {
                    FirstName = "Ragnar",
                    LastName = "Ragnarsson",
                    UserName = "ragnar@gmail.com",
                    Email = "ragnar@gmail.com",
                },
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "0Comma5!");
                if (user.Email == "michael@gmail.com")
                    await userManager.AddToRoleAsync(user, "Admin");
                else
                    await userManager.AddToRoleAsync(user, "User");
            }
        }

        if (context.ToDoLists.Any())
            return;

        var todos = new List<ToDoList>
        {
            new() { Title = "Handla", Content = "Stock snus, havremjölk, gurka" },
            new() { Title = "Tenta", Content = "tenta 2025-09-25" },
            new() { Title = "Kostschema", Content = "Saft och bullar" },
        };

        context.ToDoLists.AddRange(todos);
        await context.SaveChangesAsync();

        var firstUser = userManager.Users.FirstOrDefault();
        var firstTodo = context.ToDoLists.FirstOrDefault();

        if (firstUser != null && firstTodo != null && !context.UserToDoLists.Any())
        {
            context.UserToDoLists.Add(
                new UserToDoList { UserId = firstUser.Id, ToDoListId = firstTodo.Id }
            );
            await context.SaveChangesAsync();
        }
    }
}
