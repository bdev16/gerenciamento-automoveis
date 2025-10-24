using MinimalApi.Domain.Entities;

namespace Test.Mocks;

public class VehicleServiceMock : IVehicleService
{
    private static List<Vehicle> vehicles = new List<Vehicle>
    {
        new Vehicle
        {
            Nome = "teste",
            Marca = "testeIndus",
            Ano = 2000
        }
    };
    public List<Vehicle> All(int? page, string? nome = null, string? marca = null)
    {
        return vehicles;
    }

    public void Delete(Vehicle vechicle)
    {
        vehicles.Remove(vechicle);
    }

    public void Include(Vehicle vehicle)
    {
        vehicles.Add(vehicle);
    }

    public Vehicle? SearchForId(int id)
    {
        return vehicles.Find(vehicle => vehicle.Id == id);
    }

    public void Update(Vehicle vehicle)
    {
        var vehicleBase = vehicles.Find(vehicleBase => vehicleBase.Id == vehicle.Id);
        vehicleBase?.Nome = vehicle.Nome;
        vehicleBase?.Marca = vehicle.Marca;
        vehicleBase?.Ano = vehicle.Ano;
    }
}