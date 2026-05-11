using GardenApi.Domain.Models;

namespace GardenApi.Domain.Interfaces;

public interface IGardenRepository : IRepository<Garden>
{
    Task<Garden?> GetByIdWithItemsAsync(int id);
    Task<IEnumerable<Garden>> GetAllWithItemsAsync();
}
