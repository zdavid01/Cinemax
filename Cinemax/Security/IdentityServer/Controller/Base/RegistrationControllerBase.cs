using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Identity;
namespace IdentityServer.Controller.Base;

public class RegistrationControllerBase : ControllerBase
{
    protected readonly ILogger<AuthenticationController> _logger;
    protected readonly IMapper _mapper;
    protected readonly UserManager<User> _userManager;
    protected readonly RoleManager<IdentityRole> _roleManager;

    public RegistrationControllerBase(ILogger<AuthenticationController> logger, IMapper mapper,
        UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    protected async Task<IActionResult> RegisterNewUserWithRoles(NewUserDto newUserDto, IEnumerable<string> roles)
    {
        var user = _mapper.Map<User>(newUserDto);

        var result = await _userManager.CreateAsync(user, newUserDto.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Successfully registered new user: {user.UserName}");

        foreach (var role in roles)
        {
            var roleExist = await _roleManager.RoleExistsAsync(role);
            if (roleExist)
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation($"Added role: {role} to user: {user.UserName}");
            }
            else
            {
                _logger.LogInformation($"Role {role} does not exists.");
            }
        }

        return Ok();
    }
}