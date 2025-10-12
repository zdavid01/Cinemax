using AutoMapper;
using IdentityServer.DTOs;
using IdentityServer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Controller;

[Route("api/v1/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;

    public UserController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
    {
        _userManager = userManager;
         _roleManager = roleManager;
         _mapper = mapper;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users")]
    public async Task<ActionResult> GetUsers()
    {
        var users = await _userManager.Users.ToListAsync();
        return Ok(_mapper.Map<IEnumerable<UserDetails>>(users));
    }

    [Authorize(Roles = "Admin,Buyer")]
    [HttpGet("users/{username}")]
    public async Task<ActionResult> GetUser(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(_mapper.Map<UserDetails>(user));
    }

    [Authorize(Roles = "Admin,Buyer")]
    [HttpPost("upgrade-to-premium")]
    public async Task<ActionResult> UpgradeToPremium([FromBody] UpgradePremiumRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Username);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        if (user.IsPremium)
        {
            return BadRequest(new { message = "User is already premium" });
        }

        // Update the user's premium status
        user.IsPremium = true;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok(new { 
                message = "Successfully upgraded to premium", 
                isPremium = true 
            });
        }

        return BadRequest(new { message = "Failed to upgrade user" });
    }
    
    [Authorize(Roles = "Admin,Buyer")]
    [HttpGet("{username}/isAdmin")]
    public async Task<ActionResult> IsAdmin(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        var roles = await _userManager.GetRolesAsync(user);
        bool isAdmin = roles.Contains("Admin");

        return Ok(new { isAdmin });
    }
}



public class UpgradePremiumRequest
{
    public string Username { get; set; } = string.Empty;
}