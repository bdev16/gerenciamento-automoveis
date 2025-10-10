using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class VehicleTest
{
    [TestMethod]
    public void TestGetSetPropieties()
    {
        // Arrange
        var vehicle = new Vehicle();

        // Act
        vehicle.Id = 1;
        vehicle.Nome = "Argo";
        vehicle.Marca = "Fiat";
        vehicle.Ano = 2018;

        // Assert
        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Argo", vehicle.Nome);
        Assert.AreEqual("Fiat", vehicle.Marca);
        Assert.AreEqual(2018, vehicle.Ano);
    }
}