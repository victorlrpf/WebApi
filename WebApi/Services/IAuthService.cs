using BackendExample.Models;

namespace BackendExample.Services;

public interface IAuthService
{
    Task<UserRecord?> RegisterAsync(RegisterRequest request);
    Task<string?> LoginAsync(LoginRequest request);
}
