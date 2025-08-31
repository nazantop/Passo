using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PassoCourseApp.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        var connectionStr = Environment.GetEnvironmentVariable("PASSO_DB")
                   ?? "Host=ep-calm-butterfly-a90zf0gs-pooler.gwc.azure.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=npg_4TM6cVmkCNSY;Ssl Mode=Require;Trust Server Certificate=true";


        optionsBuilder.UseNpgsql(connectionStr);
        return new AppDbContext(optionsBuilder.Options);
    }
}