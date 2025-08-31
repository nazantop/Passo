namespace PassoCourseApp.Application.DTO;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName, string Role); 
public record LoginRequest(string Email, string Password);


public class AuthResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName { get; set; } = "";
    public List<string> Roles { get; set; } = [];
    public string AccessToken { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
}