using AutoMapper;
using IdentityServer.Controller.Base;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controller;

[Route("api/v1/[controller]")]
[ApiController]
public class AuthenticationController : RegistrationControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(ILogger<AuthenticationController> logger, IMapper mapper,
        UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IAuthenticationService authService) : base(logger, mapper, userManager,
        roleManager)
    {
        _authenticationService = authService;
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> RegisterBuyer([FromBody] NewUserDto newUser)
    {
        return await RegisterNewUserWithRoles(newUser, new string[]{"Buyer"});
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RegisterAdministrator([FromBody] NewUserDto newUser)
    {
        return await RegisterNewUserWithRoles(newUser, new string[]{"Admin"});
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] UserCredentialsDto userCredentials)
    {
        var user = await _authenticationService.ValidateUser(userCredentials);
        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(await _authenticationService.CreateAuthenticationModel(user));
    }

    [HttpPost("[action]")]
    public async Task<ActionResult<AuthenticationModel>> Refresh([FromBody] RefreshTokenModel refreshTokenCredentials)
    {
        var user = await _userManager.FindByNameAsync(refreshTokenCredentials.UserName);
        if (user == null)
        {
            _logger.LogInformation($"User {refreshTokenCredentials.UserName} not found");
            return Forbid();
        }
        
        var refreshToken = user.RefreshTokens.FirstOrDefault(r => r.Token == refreshTokenCredentials.RefreshToken);
        if (refreshToken == null)
        {
            _logger.LogInformation($"Refreshing token failed. The refresh token is not found");
            return Unauthorized();
        }

        if (refreshToken.ExpiryTime < DateTime.Now)
        {
            _logger.LogInformation($"Refreshing token failed. The refresh token has expired");
            return Unauthorized();
        }
        
        return Ok(await _authenticationService.CreateAuthenticationModel(user));
    }

    [Authorize]
    [HttpPost("[action]")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenModel refreshTokenCredentials)
    {
        var user = await _userManager.FindByNameAsync(refreshTokenCredentials.UserName);
        if (user == null)
        {
            _logger.LogInformation($"Logout failed. User {refreshTokenCredentials.UserName} not found");
            return Forbid();
        }
        await _authenticationService.RemoveRefreshToken(user, refreshTokenCredentials.RefreshToken);
        return Accepted();
    }
}