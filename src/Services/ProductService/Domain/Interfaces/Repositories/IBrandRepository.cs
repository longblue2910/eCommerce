using Domain.Aggregates;
using MongoDB.Bson;

namespace Domain.Repositories;

public interface IBrandRepository
{
    Task AddAsync(Brand brand);
    Task UpdateAsync(Brand brand);
    Task DeleteAsync(ObjectId id);
    Task<Brand?> GetByIdAsync(ObjectId id);
    Task<List<Brand>> GetAllAsync();
    Task<bool> IsNameExistsAsync(string name);
}
