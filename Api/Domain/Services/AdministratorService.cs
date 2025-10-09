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

    public Administrator? SearchForId(int id)
    {
        return _context.Administrators.Where(administrator => administrator.Id == id).FirstOrDefault();
    }

    public List<Administrator> All(int? page)
    {
        var query = _context.Administrators.AsQueryable();

        int itensForPage = 10;

        if (page != null)
            query = query.Skip(((int)page - 1) * itensForPage).Take(itensForPage);

        return query.ToList();
    }

    public Administrator Include(Administrator administrator)
    {
        _context.Administrators.Add(administrator);
        _context.SaveChanges();

        return administrator;
    }

    Administrator? IAdministratorService.Login(LoginDTO loginDTO)
    {
        var adm = _context.Administrators.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
    }
}