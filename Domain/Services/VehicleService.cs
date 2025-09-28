using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Infrastructure.Db;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;
    public VehicleService(AppDbContext context)
    {
        _context = context;
    }
    public List<Vehicle> All(int? page, string? nome = null, string? marca = null)
    {
        var query = _context.Vehicles.AsQueryable();
        if (!string.IsNullOrEmpty(nome))
        {
            query = query.Where(vehicle => EF.Functions.Like(vehicle.Nome.ToLower(), $"%{nome.ToLower()}%"));
        }

        int itensForPage = 10;

        if(page != null)
            query = query.Skip(((int)page - 1) * itensForPage).Take(itensForPage);

        return query.ToList();
    }

    public void Delete(Vehicle vechicle)
    {
        _context.Vehicles.Remove(vechicle);
        _context.SaveChanges();
    }

    public void Include(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }

    public Vehicle? SearchForId(int id)
    {
        return _context.Vehicles.Where(vehicle => vehicle.Id == id).FirstOrDefault();
    }

    public void Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}