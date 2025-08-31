using api.Data;
using api.Services;
using api.ViewModels;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly HtmlSanitizer _htmlSanitizer = new();
    private readonly TokenService _tokenService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AccountsController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        TokenService tokenService
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserViewModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        model.Email = _htmlSanitizer.Sanitize(model.Email);
        model.FirstName = _htmlSanitizer.Sanitize(model.FirstName);
        model.LastName = _htmlSanitizer.Sanitize(model.LastName);

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, "User");
        return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<ActionResult> LoginUser(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return ValidationProblem();

        model.Email = _htmlSanitizer.Sanitize(model.Email);
        model.Password = _htmlSanitizer.Sanitize(model.Password);

        var user = await _signInManager.UserManager.FindByEmailAsync(model.Email);
        if (user == null)
            return BadRequest("Fel e-post eller lösenord.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
        if (!result.Succeeded)
            return BadRequest("Fel e-post eller lösenord.");

        var token = await _tokenService.CreateToken(user);
        var roles = await _signInManager.UserManager.GetRolesAsync(user);

        Response.Cookies.Append(
            "jwt",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            }
        );

        return Ok(
            new
            {
                success = true,
                email = user.Email,
                token,
                role = roles.FirstOrDefault(),
            }
        );
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }
}
