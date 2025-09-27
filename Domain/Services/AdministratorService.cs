using System.Data.Common;
using MinimalApi.Domain.Entities;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Dominio.Services;

public class AdministratorService : IAdministratorService
{
    private readonly AppDbContext _context;
    public AdministratorService(AppDbContext context)
    {
        _context = context;
    }

    Administrator? IAdministratorService.Login(LoginDTO loginDTO)
    {
        var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}