using Domain.Entities;
using MongoDB.Bson;

namespace Domain.Repositories;

public interface ISupplierRepository
{
    Task AddAsync(Supplier supplier);
    Task UpdateAsync(Supplier supplier);
    Task DeleteAsync(ObjectId id);
    Task<Supplier?> GetByIdAsync(ObjectId id);
    Task<List<Supplier>> GetAllAsync();
}
