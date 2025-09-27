using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministratorService
{
    Administrator? Login(LoginDTO loginDTO);
}