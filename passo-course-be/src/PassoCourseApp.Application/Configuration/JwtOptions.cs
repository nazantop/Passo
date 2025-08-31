namespace PassoCourseApp.Application.Configuration;

public class JwtOptions
{
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public int ExpMinutes { get; set; } = 60;
}