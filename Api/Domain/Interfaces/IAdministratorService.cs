using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministratorService
{
    Administrator? Login(LoginDTO loginDTO);
    Administrator Include(Administrator administrator);
    Administrator? SearchForId(int id);
    List<Administrator> All(int? page);
}