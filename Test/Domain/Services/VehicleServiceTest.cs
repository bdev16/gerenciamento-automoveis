using System.Reflection;
using Microsoft.Extensions.Configuration;
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
}