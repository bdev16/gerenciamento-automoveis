using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlserver")
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (MinimalApi.DTOs.LoginDTO loginDTO, IAdministratorService administratorService) => {
    if (administratorService.Login(loginDTO) != null) {
        return Results.Ok("Login com sucesso!");
    }
    else
        return Results.Unauthorized();
});

app.Run();