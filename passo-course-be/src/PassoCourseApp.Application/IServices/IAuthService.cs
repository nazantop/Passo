using PassoCourseApp.Application.DTO;

namespace PassoCourseApp.Application.IServices;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
}