using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

namespace Test.Domain.Services;

[TestClass]
public class VehicleServiceTest
{
    private AppDbContext CreateContextTest()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

        var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new AppDbContext(configuration);
    }

    [TestMethod]
    public void TestSaveVehicleToDb()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles;");

        var vehicle = new Vehicle();
        vehicle.Nome = "teste";
        vehicle.Marca = "testeteste";
        vehicle.Ano = 2000;

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Include(vehicle);

        // Assert
        Assert.AreEqual(1, vehicleService.All(1).Count());
    }

    [TestMethod]
    public void TestSearchVehicleForIdToBd()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles;");

        var vehicle = new Vehicle();
        vehicle.Nome = "teste";
        vehicle.Marca = "testeteste";
        vehicle.Ano = 2000;

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Include(vehicle);
        var vehicleResult = vehicleService.SearchForId(1);

        // Assert
        Assert.AreEqual(1, vehicleResult?.Id);
    }

    [TestMethod]
    public void TestSearchAllVehiclesToBd()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles;");

        var vehicle = new Vehicle();
        vehicle.Nome = "teste";
        vehicle.Marca = "testeteste";
        vehicle.Ano = 2000;

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Include(vehicle);

        // Assert
        Assert.AreEqual(1, vehicleService.All(1).Count());
    }

    [TestMethod]
    public void UpdateVehicleToDB()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles;");

        var vehicle = new Vehicle();
        vehicle.Nome = "teste";
        vehicle.Marca = "testeteste";
        vehicle.Ano = 2000;

        var vehicleNome = vehicle.Nome;
        var vehicleMarca = vehicle.Marca;
        var vehicleAno = vehicle.Ano;

        var vehicleDTO = new VehicleDTO();
        vehicleDTO.Nome = "teste modificado";
        vehicleDTO.Marca = "testeteste modificado";
        vehicleDTO.Ano = 2010;

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Include(vehicle);

        vehicle.Nome = vehicleDTO.Nome;
        vehicle.Marca = vehicleDTO.Marca;
        vehicle.Ano = vehicleDTO.Ano;

        vehicleService.Update(vehicle);

        // Assert
        Assert.IsFalse(vehicle.Nome == vehicleNome);
        Assert.IsFalse(vehicle.Marca == vehicleMarca);
        Assert.IsFalse(vehicle.Ano == vehicleAno);
    
    }

    [TestMethod]
    public void DeleteVehicleToBD()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Vehicles;");

        var vehicle = new Vehicle();
        vehicle.Nome = "teste";
        vehicle.Marca = "testeteste";
        vehicle.Ano = 2000;

        var vehicleTwo = new Vehicle();
        vehicleTwo.Nome = "teste02";
        vehicleTwo.Marca = "testeteste02";
        vehicleTwo.Ano = 2002;

        var vehicleService = new VehicleService(context);

        // Act
        vehicleService.Include(vehicle);
        vehicleService.Include(vehicleTwo);

        vehicleService.Delete(vehicle);

        var vehicleResult = vehicleService.SearchForId(1);

        // Assert
        Assert.IsNull(vehicleResult);

    }
}