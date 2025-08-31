using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PassoCourseApp.Application.Configuration;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Domain.Entities;
using PassoCourseApp.Infrastructure.Data;
using PassoCourseApp.Infrastructure.Services;
using PassoCourseApp.UnitTest.Helpers;

namespace PassoCourseApp.UnitTest;

public class AuthServiceTests(DbFixture fx) : IClassFixture<DbFixture>
{
    private static IOptions<JwtOptions> Jwt() => Options.Create(new JwtOptions
    {
        SecretKey = "supersecret_supersecret_supersecret_32",
        Issuer = "PassoCourseApp",
        Audience = "PassoCourseAppAudience",
        ExpMinutes = 60
    });

    private AppDbContext NewDb() => fx.CreateContext();
    private AuthService NewSvc(AppDbContext db) => new(db, new PasswordHasher<User>(), Jwt());
    private Task<AuthResponse> Reg(AuthService svc, string email, string password, string first = "N", string last = "S", string role = "User")
        => svc.RegisterAsync(new RegisterRequest(email, password, first, last, role));

    [Fact]
    public async Task Register_AssignsRoleAndCreatesToken()
    {
        await using var db = NewDb();
        var svc = NewSvc(db);

        var req = new RegisterRequest("x@y.com", "P@ssw0rd!", "N", "S", "User");
        var res = await svc.RegisterAsync(req);

        Assert.Equal(req.Email, res.Email);
        Assert.Contains("User", res.Roles);
        Assert.False(string.IsNullOrWhiteSpace(res.AccessToken));
        var user = await db.Users.Include(u=>u.UserRoles).ThenInclude(ur=>ur.Role).FirstAsync(u=>u.Email==req.Email);
        Assert.Contains(user.UserRoles.Select(r=>r.Role.Name), r => r == "User");
    }

    [Fact]
    public async Task Register_DuplicateEmail_Throws()
    {
        await using var db = NewDb();
        var svc = NewSvc(db);
        await Reg(svc, "a@b.com", "X", "A", "B", "User");
        await Assert.ThrowsAsync<InvalidOperationException>(() => Reg(svc, "a@b.com", "Y", "C", "D", "User"));
    }

    [Fact]
    public async Task Login_ReturnsToken()
    {
        await using var db = NewDb();
        var svc = NewSvc(db);
        await Reg(svc, "l@t.com", "Zz_1!", "L", "T", "User");
        var res = await svc.LoginAsync(new LoginRequest("l@t.com", "Zz_1!"));
        Assert.False(string.IsNullOrWhiteSpace(res.AccessToken));
    }

    [Fact]
    public async Task Login_WrongPassword_Throws()
    {
        await using var db = NewDb();
        var svc = NewSvc(db);
        await Reg(svc, "w@p.com", "Ok_1!", "W", "P", "User");
        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.LoginAsync(new LoginRequest("w@p.com", "bad")));
    }
}
