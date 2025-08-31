using api.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpPost]
    public async Task<ActionResult> AddRole(RolePostViewModel model)
    {
        var role = new IdentityRole
        {
            Name = model.RoleName,
            NormalizedName = model.RoleName.ToUpper(),
        };
        await _roleManager.CreateAsync(role);

        return StatusCode(201);
    }
}
