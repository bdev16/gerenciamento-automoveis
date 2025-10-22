using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using MinimalApi.Domain.ModelViews;
using MinimalApi.DTOs;
using Test.Helper;

namespace Test.Request;

[TestClass]
public class AdministratorRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestLoginRequest()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "administrator@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var response = await Setup.client.PostAsync("/Administrators/login", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var administratorLogged = JsonSerializer.Deserialize<AdministratorLoggad>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });


        Assert.IsNotNull(administratorLogged?.Email ?? "");
        Assert.IsNotNull(administratorLogged?.Perfil ?? "");
        Assert.IsNotNull(administratorLogged?.Token ?? "");
    }
}