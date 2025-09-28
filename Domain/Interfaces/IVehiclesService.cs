using MinimalApi.Domain.Entities;

public interface IVehicleService
{
    List<Vehicle> All(int page, string? nome = null, string? marca = null);
    Vehicle? SearchForId(int id);
    void Include(Vehicle vehicle);
    void Update(Vehicle vehicle);
    void Delete(Vehicle vechicle);
}