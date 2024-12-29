using System.ComponentModel.DataAnnotations;

namespace IdentityServer.DTOs;

public class UserCredentialsDto
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}