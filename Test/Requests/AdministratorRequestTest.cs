using System.Net;
using System.Net.Http.Headers;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using MinimalApi.Domain.Enums;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;
using Test.Helper;
using Test.Helpers;

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

    [TestMethod]
    public async Task TestSaveAdministratorRequest()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "administrator@teste.com",
            Senha = "123456"
        };

        var contentLogin = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        var administratorDTO = new AdministratorDTO
        {
            Email = "administratorRegisterTest@teste.com",
            Senha = "123456",
            Perfil = Profile.Administrator
        };

        var contentRegister = new StringContent(JsonSerializer.Serialize(administratorDTO), Encoding.UTF8, "Application/json");

        var administratorServiceMock = new AdministratorServiceMock();
        // Act
        var responseLogin = await Setup.client.PostAsync("/Administrators/login", contentLogin);

        // administratorServiceMock.Include(administratorServiceMock.DTOtoAdministrator(administratorDTO));

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, responseLogin.StatusCode);

        var result = await responseLogin.Content.ReadAsStringAsync();
        var administratorLogged = JsonSerializer.Deserialize<AdministratorLoggad>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(administratorLogged?.Email ?? "");
        Assert.IsNotNull(administratorLogged?.Perfil ?? "");
        Assert.IsNotNull(administratorLogged?.Token ?? "");

        Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", administratorLogged?.Token);

        var responseRegister = await Setup.client.PostAsync("/Administrators", contentRegister);

        Assert.AreEqual(HttpStatusCode.Created, responseRegister.StatusCode);

        var resultRegisterRequest = await responseRegister.Content.ReadAsStringAsync();
        var administratorModelView = JsonSerializer.Deserialize<AdministratorModelView>(resultRegisterRequest, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.AreEqual(administratorDTO.Email, administratorModelView?.Email);
        Assert.AreEqual(administratorDTO.Perfil.ToString(), administratorModelView?.Profile);

    }

    [TestMethod]
    public async Task TestGetEspecificAdminsitratorRequest()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "administrator@teste.com",
            Senha = "123456"
        };

        var contentLogin = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var responseLoginRequest = await Setup.client.PostAsync("/Administrators/login", contentLogin);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, responseLoginRequest.StatusCode);

        var resultResponseLoginRequest = await responseLoginRequest.Content.ReadAsStringAsync();
        var administratorLogged = JsonSerializer.Deserialize<AdministratorLoggad>(resultResponseLoginRequest, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(administratorLogged?.Email ?? "");
        Assert.IsNotNull(administratorLogged?.Perfil ?? "");
        Assert.IsNotNull(administratorLogged?.Token ?? "");

        Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", administratorLogged?.Token ?? "");

        var responseGetAdministratorRequest = await Setup.client.GetAsync("/administrators/1");

        Assert.AreEqual(HttpStatusCode.OK, responseGetAdministratorRequest.StatusCode);
    }

    [TestMethod]
    public async Task TestGetAllAdministratorRequest()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "administrator@teste.com",
            Senha = "123456"
        };

        var contentLogin = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        // Act
        var responseLoginRequest = await Setup.client.PostAsync("/Administrators/login", contentLogin);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, responseLoginRequest.StatusCode);

        var resultResponseLoginRequest = await responseLoginRequest.Content.ReadAsStringAsync();
        var administratorLogged = JsonSerializer.Deserialize<AdministratorLoggad>(resultResponseLoginRequest, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(administratorLogged?.Email ?? "");
        Assert.IsNotNull(administratorLogged?.Perfil ?? "");
        Assert.IsNotNull(administratorLogged?.Token ?? "");

        Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", administratorLogged?.Token);

        var responseGetAllAdministratorsRequest = await Setup.client.GetAsync("/administrators");

        var resultResponseGetAllAdministratorRequest = await responseGetAllAdministratorsRequest.Content.ReadAsStringAsync();
        var administrators = JsonSerializer.Deserialize<List<AdministratorModelView>>(resultResponseGetAllAdministratorRequest, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.AreNotEqual(0, administrators?.Count);
    }
}