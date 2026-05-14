using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesManager.WebAPI.Controllers;

/// <summary>
/// Account controller
/// </summary>
[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IJwtService _jwtService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="roleManager"></param>
    /// <param name="jwtService"></param>
    public AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<ApplicationRole> roleManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
    }
    /// <summary>
    /// Register a user in the database
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDto userDto)
    {
        if (ModelState.IsValid == false)
        {
            string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => 
                v.Errors).Select(e => e.ErrorMessage));
            return Problem(errorMessage);
        }

        ApplicationUser appUser = new ApplicationUser()
        {
            Email = userDto.Email,
            PhoneNumber = userDto.PhoneNumber,
            UserName = userDto.Email,
            Name = userDto.Name
        };

        IdentityResult result = await _userManager.CreateAsync(appUser, userDto.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(appUser, isPersistent: false);

            //return Ok(appUser);

            var authenticationResponse = _jwtService.CreateJwtToken(appUser);
            appUser.RefreshToken = authenticationResponse.RefreshToken;
            appUser.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;
            await _userManager.UpdateAsync(appUser);
            
            return Ok(authenticationResponse);
        }
        else
        {
            return Problem(string.Join(" | ", result.Errors.Select(e => e.Description)));
        }
    }

    /// <summary>
    /// sign a user in
    /// </summary>
    /// <param name="userDto"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> PostLogin(LoginDto userDto)
    {
        if (ModelState.IsValid == false)
        {
            string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v =>
                v.Errors).Select(e => e.ErrorMessage));
            return Problem(errorMessage);
        }

        var result = await _signInManager.PasswordSignInAsync(userDto.Email, userDto.Password, 
            isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            ApplicationUser? appUser = await _userManager.FindByEmailAsync(userDto.Email);
            if (appUser == null) // unlikely to happen
            {
                return NoContent();
            }
            // return Ok(new { Name = appUser.Name, Email = appUser.Email });
            
            var authenticationResponse = _jwtService.CreateJwtToken(appUser);
            appUser.RefreshToken = authenticationResponse.RefreshToken;
            appUser.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;
            await _userManager.UpdateAsync(appUser);

            return Ok(authenticationResponse);     
        }
        else
        {
            return Problem("Invalid credentials");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("logout")]
    public async Task<IActionResult> GetLogout()
    {
        await _signInManager.SignOutAsync();

        return NoContent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("generate-new-jwt-token")]
    public async Task<IActionResult> GenerateNewAccessToken(TokenDto token)
    {
        if (token == null) return BadRequest("Invalid client request");

        ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(token.Token);
        string? email = principal?.FindFirstValue(ClaimTypes.Email);

        if (principal == null || string.IsNullOrEmpty(email))
        {
            return BadRequest("Invalid token");
        }

        ApplicationUser? user = await _userManager.FindByEmailAsync(email);

        if (user == null || user.RefreshToken != token.RefreshToken
            || user.RefreshTokenExpiration <= DateTime.Now)
        {
            return BadRequest("Invalid refresh token");
        }
        var authenticationResponse = _jwtService.CreateJwtToken(user);
        user.RefreshToken = authenticationResponse.RefreshToken;
        user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;
        await _userManager.UpdateAsync(user);
        return Ok(authenticationResponse);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ValidateEmail(string email)
    {
        ApplicationUser? appUser = await _userManager.FindByEmailAsync(email);

        return Ok(appUser == null);
    }
}
