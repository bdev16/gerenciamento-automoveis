using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensibility;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

# region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();

if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui:"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("sqlserver")
    );
});

var app = builder.Build();
# endregion

# region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
# endregion

# region Administrators
string GenerateTokenJwt(Administrator administrator)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrator.Email),
        new Claim("Profile", administrator.Perfil),
        new Claim(ClaimTypes.Role, administrator.Perfil)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/Administrators/login", (MinimalApi.DTOs.LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    var administrator = administratorService.Login(loginDTO);
    if (administrator != null)
    {
        string token = GenerateTokenJwt(administrator);
        return Results.Ok(new AdministratorLoggad{
            Email = administrator.Email,
            Perfil = administrator.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administrators");

app.MapGet("/administrators", (int? page, IAdministratorService administratorService) =>
{
    var administrators = new List<AdministratorModelView>();
    var administratorsGet = administratorService.All(page);
    foreach (var adm in administratorsGet)
    {
        administrators.Add(new AdministratorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Profile = adm.Perfil
        });
    }
    return Results.Ok(administrators);
}).RequireAuthorization().WithTags("Administrators");

app.MapGet("/administrators/{id}", (int id, IAdministratorService administratorService) =>
{
    var administrator = administratorService.SearchForId(id);
    if (administrator == null) return Results.NotFound();
    return Results.Ok(new AdministratorModelView
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Perfil
    });
}).RequireAuthorization().WithTags("Administrators");

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

    return Results.Created($"/administrador/{administrator.Id}", new AdministratorModelView
    {
        Id = administrator.Id,
        Email = administrator.Email,
        Profile = administrator.Perfil
    });
}).RequireAuthorization().WithTags("Administrators");

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

app.UseAuthentication();
app.UseAuthorization();

app.Run();
# endregion