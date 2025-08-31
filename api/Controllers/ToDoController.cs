using api.Data;
using api.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ToDoController : ControllerBase
{
    private readonly DataContext _context;
    private readonly HtmlSanitizer _htmlSanitizer;

    public ToDoController(DataContext context, HtmlSanitizer htmlSanitizer)
    {
        _htmlSanitizer = htmlSanitizer;
        _context = context;
    }

    [HttpGet()]
    public async Task<ActionResult<IList<ToDoListViewModel>>> ListAll()
    {
        var userId = User
            .Claims.First(c => c.Type == "sub" || c.Type.EndsWith("nameidentifier"))
            .Value;

        var response = await _context.ToDoLists.ToListAsync();
        var lists = await _context
            .UserToDoLists.Where(x => x.UserId == userId)
            .Select(x => new ToDoListViewModel
            {
                Id = x.ToDoList.Id,
                Title = x.ToDoList.Title,
                Content = x.ToDoList.Content,
                TimeCreated = x.ToDoList.TimeCreated,
            })
            .ToListAsync();

        return Ok(lists);
    }

    [HttpPost()]
    public async Task<ActionResult> AddList([FromBody] ToDoListPostViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User
            .Claims.First(c => c.Type == "sub" || c.Type.EndsWith("nameidentifier"))
            .Value;

        var list = new ToDoList
        {
            Title = _htmlSanitizer.Sanitize(model.Title),
            Content = _htmlSanitizer.Sanitize(model.Content),
            TimeCreated = DateTime.Now,
        };

        await _context.AddAsync(list);
        await _context.SaveChangesAsync();

        var userToDo = new UserToDoList { UserId = userId, ToDoListId = list.Id };
        await _context.UserToDoLists.AddAsync(userToDo);
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteToDo(int id)
    {
        var list = await _context.ToDoLists.FindAsync(id);
        if (list == null)
            return NotFound();

        _context.ToDoLists.Remove(list);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateToDo(int id, [FromBody] ToDoListPostViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User
            .Claims.First(c => c.Type == "sub" || c.Type.EndsWith("nameidentifier"))
            .Value;

        var userToDo = await _context
            .UserToDoLists.Include(x => x.ToDoList)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ToDoListId == id);

        if (userToDo == null)
            return NotFound();

        userToDo.ToDoList.Title = _htmlSanitizer.Sanitize(model.Title);
        userToDo.ToDoList.Content = _htmlSanitizer.Sanitize(model.Content);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
