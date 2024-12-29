using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public bool IsPremium { get; set; } = false;
    
    public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}