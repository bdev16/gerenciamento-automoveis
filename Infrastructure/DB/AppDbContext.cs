using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.Db;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configurationAppSettings;
    public AppDbContext(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }
    public DbSet<Administrator> Administrators { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasData(
            new Administrator
            {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConection = _configurationAppSettings.GetConnectionString("sqlserver")?.ToString();
            if (!string.IsNullOrEmpty(stringConection))
            {
                optionsBuilder.UseSqlServer(stringConection);
            }
        }
    }
}