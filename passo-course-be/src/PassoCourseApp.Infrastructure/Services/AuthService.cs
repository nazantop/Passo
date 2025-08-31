using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PassoCourseApp.Application.Configuration;
using PassoCourseApp.Application.DTO;
using PassoCourseApp.Application.IServices;
using PassoCourseApp.Domain.Entities;
using PassoCourseApp.Infrastructure.Data;

namespace PassoCourseApp.Infrastructure.Services;

public class AuthService(AppDbContext db, IPasswordHasher<User> hasher, IOptions<JwtOptions> jwtOptions)
    : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var exists = await db.Users.AnyAsync(u => u.Email == request.Email);
        if (exists) throw new InvalidOperationException("Email already registered");


        var user = new User
        {
            Id = Guid.NewGuid(), 
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);


        await db.Users.AddAsync(user);
        await db.SaveChangesAsync();
        
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == request.Role)
                   ?? throw new InvalidOperationException("Role not found");
        db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
        await db.SaveChangesAsync();


        return await BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await db.Users
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null) throw new InvalidOperationException("Invalid credentials");


        var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed) throw new InvalidOperationException("Invalid credentials");


        return await BuildAuthResponse(user);
    }
    
    private Task<AuthResponse> BuildAuthResponse(User user)
    {
        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpMinutes);


        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );


        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.WriteToken(token);


        return Task.FromResult(new AuthResponse
        {
            UserId = user.Id,
            Email = user.Email,
            Roles = roles,
            AccessToken = jwt,
            ExpiresAt = expires
        });
    }
}