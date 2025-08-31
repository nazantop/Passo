using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using PassoCourseApp.Application.Configuration;
using PassoCourseApp.Application.IServices;
using PassoCourseApp.Domain.Entities;
using PassoCourseApp.Infrastructure.Data;
using PassoCourseApp.Infrastructure.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

OverrideJwt(builder);

builder.Services.AddControllers();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connStr));

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

builder.Services.AddSingleton<PassoCourseApp.Infrastructure.Caching.ICacheGate, PassoCourseApp.Infrastructure.Caching.CacheGate>();

builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<ICourseService>(sp =>
    new CachedCourseService(
        sp.GetRequiredService<CourseService>(),
        sp.GetRequiredService<IDistributedCache>(),
        sp.GetRequiredService<PassoCourseApp.Infrastructure.Caching.ICacheGate>()));

builder.Services.AddScoped<ProgressService>();
builder.Services.AddScoped<IProgressService>(sp =>
    new CachedProgressService(
        sp.GetRequiredService<ProgressService>(),
        sp.GetRequiredService<IDistributedCache>(),
        sp.GetRequiredService<PassoCourseApp.Infrastructure.Caching.ICacheGate>()));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(o =>
{
    o.AddPolicy("allow-all", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("allow-all");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
return;

static void OverrideJwt(WebApplicationBuilder builder)
{
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
    var jwtOpt = new JwtOptions();
    builder.Configuration.GetSection("Jwt").Bind(jwtOpt);

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.SecretKey ?? "super_secret_key"));

    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOpt.Issuer,
                ValidAudience = jwtOpt.Audience,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });

    builder.Services.AddAuthorization();

    var redisConfig = builder.Configuration["Redis:Configuration"];
    if (string.IsNullOrWhiteSpace(redisConfig))
    {
        builder.Services.AddDistributedMemoryCache();
    }
    else
    {
        var co = ConfigurationOptions.Parse(redisConfig);
        co.AbortOnConnectFail = false;
        co.ConnectTimeout = co.ConnectTimeout == 0 ? 200 : co.ConnectTimeout;
        co.SyncTimeout = co.SyncTimeout == 0 ? 400 : co.SyncTimeout;
        co.ConnectRetry = co.ConnectRetry == 0 ? 1 : co.ConnectRetry;
        builder.Services.AddStackExchangeRedisCache(o =>
        {
            o.InstanceName = builder.Configuration["Redis:InstanceName"];
            o.ConfigurationOptions = co;
        });
    }

    builder.Services.AddAuthorization();
}