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
}