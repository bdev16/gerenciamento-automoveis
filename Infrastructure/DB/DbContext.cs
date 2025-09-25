using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalApi.Domain.Entities;

namespace MinimalApi.Infrastructure.Db;

public class DbContext : DbContext
{
    public DbSet<Administrator> Administrators { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("string de conexão");
    }
}