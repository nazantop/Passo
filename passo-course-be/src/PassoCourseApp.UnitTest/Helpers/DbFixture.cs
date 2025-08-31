using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PassoCourseApp.Infrastructure.Data;

namespace PassoCourseApp.UnitTest.Helpers;

public sealed class DbFixture : IDisposable
{
    private readonly SqliteConnection _connection;
    public DbFixture()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
    }
    public AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;
        var ctx = new AppDbContext(options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        return ctx;
    }
    public void Dispose() => _connection.Dispose();
}