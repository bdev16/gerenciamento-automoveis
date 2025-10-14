using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Domain.Entities;
using MinimalApi.Dominio.Services;
using MinimalApi.Infrastructure.Db;

namespace Test.Domain.Entities;

[TestClass]
public class AdministratorServiceTest
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
    public void TestSaveAdministratorToBd()
    {

        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators;");

        var administrator = new Administrator();
        administrator.Email = "administrador@teste.com";
        administrator.Senha = "123456";
        administrator.Perfil = "Adm";

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Include(administrator);

        // Assert
        Assert.AreEqual(1, administratorService.All(1).Count());

    }

    [TestMethod]
    public void TestSearchAdministratorForIdToBd()
    {
        // Arrange
        var context = CreateContextTest();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administrators;");

        var administrator = new Administrator();
        administrator.Email = "administrator@teste.com";
        administrator.Senha = "123456";
        administrator.Perfil = "Adm";

        var administratorService = new AdministratorService(context);

        // Act
        administratorService.Include(administrator);
        var administratorResult = administratorService.SearchForId(administrator.Id);

        // Assert
        Assert.AreEqual(1, administratorResult?.Id);
    }

}