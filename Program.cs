using Microsoft.EntityFrameworkCore;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlserver")
    );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (MinimalApi.DTOs.LoginDTO loginDTO) => {
    if (loginDTO.Email == "adm@teste.com" && loginDTO.Senha == "123456") {
        return Results.Ok("Login com sucesso!");
    }
    else
        return Results.Unauthorized();
});

app.Run();