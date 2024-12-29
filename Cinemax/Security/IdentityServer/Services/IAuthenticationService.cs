using IdentityServer.DTOs;
using IdentityServer.Entities;

namespace IdentityServer.Services;

public interface IAuthenticationService
{
    Task<User> ValidateUser(UserCredentialsDto userCredentialsDto);
    Task<AuthenticationModel> CreateAuthenticationModel(User user);
    
    Task RemoveRefreshToken(User user, string refreshToken);
}