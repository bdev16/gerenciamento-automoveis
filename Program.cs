using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensibility;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

# region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

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
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
# endregion

# region Administrators
app.MapPost("/Administrators/login", (MinimalApi.DTOs.LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso!");
    }
    else
        return Results.Unauthorized();
}).WithTags("Administrators");

app.MapGet("/administrators", (int? page, IAdministratorService administratorService) =>
{
    return Results.Ok(administratorService.All(page));
}).WithTags("Administrators");

app.MapPost("/administrators", (AdministratorDTO administratorDTO, IAdministratorService administratorService) =>
{
    var validation = new ValidationErros
    {
        Mensages = new List<string>()
    };

    if (string.IsNullOrEmpty(administratorDTO.Email))
        validation.Mensages.Add("Email não pode ser vazio");

    if (string.IsNullOrEmpty(administratorDTO.Senha))
        validation.Mensages.Add("Senha não pode ser vazia");

    if (administratorDTO.Perfil == null)
        validation.Mensages.Add("Perfil não pode ser vazio");

    if (validation.Mensages.Count > 0)
        return Results.BadRequest(validation);

    var administrator = new Administrator
    {
        Email = administratorDTO.Email,
        Senha = administratorDTO.Senha,
        Perfil = administratorDTO.Perfil.ToString() ?? Profile.Editor.ToString()
    };

    administratorService.Include(administrator);

    return Results.Created($"/administrador/{administrator.Id}", administrator);
}).WithTags("Administrators");

# endregion

# region Vehicles
ValidationErros validateDTO(VehicleDTO vehicleDTO)
{
    var validation = new ValidationErros
    {
        Mensages = new List<string>()
    };

    if (string.IsNullOrEmpty(vehicleDTO.Nome))
        validation.Mensages.Add("O nome não pode ser vazio.");

    if (string.IsNullOrEmpty(vehicleDTO.Marca))
        validation.Mensages.Add("A marca não pode ficar em branco");

    if (vehicleDTO.Ano < 1950)
        validation.Mensages.Add("Veiculo muito antigo, aceito somente anos superiores a 1950");

    return validation;
}

app.MapPost("/vehicles", (VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var validation = validateDTO(vehicleDTO);

    if (validation.Mensages.Count > 0)
        return Results.BadRequest(validation);

    var vehicle = new Vehicle
    {
        Nome = vehicleDTO.Nome,
        Marca = vehicleDTO.Marca,
        Ano = vehicleDTO.Ano
    };
    vehicleService.Include(vehicle);

    return Results.Created($"/vehicle/{vehicle.Id}", vehicle);
}).WithTags("Vehicles");

app.MapGet("/vehicles", (int? page, IVehicleService vehicleService) =>
{
    var vehicles = vehicleService.All(page);

    return Results.Ok(vehicles);
}).WithTags("Vehicles");

app.MapGet("/vehicles/{id}", (int id, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.SearchForId(id);

    if (vehicle == null) return Results.NotFound();

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapPut("/vehicles/{id}", (int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
{
    var vehicle = vehicleService.SearchForId(id);
    if (vehicle == null) return Results.NotFound();

    var validation = validateDTO(vehicleDTO);

    if (validation.Mensages.Count > 0)
        return Results.BadRequest(validation);

    vehicle.Nome = vehicleDTO.Nome;
    vehicle.Marca = vehicleDTO.Marca;
    vehicle.Ano = vehicleDTO.Ano;

    vehicleService.Update(vehicle);

    return Results.Ok(vehicle);
}).WithTags("Vehicles");

app.MapDelete("/vehicles/{id}", (int id, IVehicleService vehicleService) =>
{
    var vechicle = vehicleService.SearchForId(id);
    if (vechicle == null) return Results.NotFound();

    vehicleService.Delete(vechicle);

    return Results.NoContent();
}).WithTags("Vehicles");
# endregion

# region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
# endregion