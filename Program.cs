using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensibility;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

# region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlserver")
    );
});

var app = builder.Build();
# endregion

# region Home
app.MapGet("/", () => Results.Json(new Home()));
# endregion

# region Administrators
app.MapPost("/login", (MinimalApi.DTOs.LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso!");
    }
    else
        return Results.Unauthorized();
});
# endregion

# region Vehicles

# endregion

# region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
# endregion