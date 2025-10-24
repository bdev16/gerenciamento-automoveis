using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Microsoft.AspNetCore.Http.Features;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.ModelViews;
using MinimalApi.DTOs;
using Test.Helper;

namespace Test.Request;

[TestClass]
public class VehicleRequestTest
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
    public async Task TestSaveVehicleRequest()
    {
        // Arrange
        var loginDTO = new LoginDTO
        {
            Email = "administrator@teste.com",
            Senha = "123456"
        };

        var contentLogin = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");

        var vehicleDTO = new VehicleDTO
        {
            Nome = "teste02",
            Marca = "testeMarca02",
            Ano = 2002
        };

        var contentVehicle = new StringContent(JsonSerializer.Serialize(vehicleDTO), Encoding.UTF8, "Application/json");

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

        var responseSaveVehicleRequest = await Setup.client.PostAsync("/vehicles", contentVehicle);

        Assert.AreEqual(HttpStatusCode.Created, responseSaveVehicleRequest.StatusCode);

        var resultResponseSaveVehicleRequest = await responseSaveVehicleRequest.Content.ReadAsStringAsync();
        var vehicleCreatedResult = JsonSerializer.Deserialize<Vehicle>(resultResponseSaveVehicleRequest, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(vehicleCreatedResult?.Nome ?? "");
        Assert.IsNotNull(vehicleCreatedResult?.Marca ?? "");
        Assert.IsNotNull(vehicleCreatedResult?.Ano ?? 0);
    }

}