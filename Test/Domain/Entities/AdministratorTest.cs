using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdministratorTest
{
    [TestMethod]
    public void TestGetSetPropieties()
    {
        // Arrange
        var administrator = new Administrator();

        // Act
        administrator.Id = 1;
        administrator.Email = "administrador@teste.com";
        administrator.Senha = "123456";
        administrator.Perfil = "Adm";

        // Assert
        Assert.AreEqual(1, administrator.Id);
        Assert.AreEqual("administrador@teste.com", administrator.Email);
        Assert.AreEqual("123456", administrator.Senha);
        Assert.AreEqual("Adm", administrator.Perfil);
    }
}