using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.Db;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    public DbContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    public DbSet<Administrator> Administrators { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConection = _configurationAppSettings.GetConnectionString("sqlserver")?.ToString();
            if (!string.IsNullOrEmpty(stringConection))
            {
                optionsBuilder.UseNpgsql(stringConection);
            }
        }
    }
}