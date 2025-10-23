using MinimalApi.Domain.Entities;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;

namespace Test.Helpers;

public class AdministratorServiceMock : IAdministratorService
{
    private static List<Administrator> administrators = new List<Administrator>()
    {
        new Administrator
        {
            Id = 1,
            Email = "administrator@teste.com",
            Senha = "123456",
            Perfil = "Adm"
        },

        new Administrator
        {
            Id = 2,
            Email = "editor@teste.com",
            Senha = "123456",
            Perfil = "Editor"
        }
    };

    public List<Administrator> All(int? page)
    {
        return administrators;
    }

    public Administrator Include(Administrator administrator)
    {
        administrator.Id = administrators.Count + 1;
        administrators.Add(administrator);
        return administrator;
    }

    public Administrator? Login(LoginDTO loginDTO)
    {
        return administrators.Find(administrator => administrator.Email == loginDTO.Email && administrator.Senha == loginDTO.Senha);
    }

    public Administrator? SearchForId(int id)
    {
        return administrators.Find(administrator => administrator.Id == id);
    }

    public Administrator DTOtoAdministrator(AdministratorDTO administratorDTO)
    {
        return new Administrator
        {
            Email = administratorDTO.Email,
            Senha = administratorDTO.Senha,
            Perfil = administratorDTO.Perfil.ToString() ?? ""
        };
    }
}