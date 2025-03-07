using Domain.Entities;
using Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoSupplierRepository(MongoDbContext context) : ISupplierRepository
{
    private readonly IMongoCollection<Supplier> _suppliers = context.GetCollection<Supplier>("Suppliers");

    public async Task AddAsync(Supplier supplier)
    {
        await _suppliers.InsertOneAsync(supplier);
    }

    public async Task UpdateAsync(Supplier supplier)
    {
        var filter = Builders<Supplier>.Filter.Eq(s => s.Id, supplier.Id);
        await _suppliers.ReplaceOneAsync(filter, supplier);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        var filter = Builders<Supplier>.Filter.Eq(s => s.Id, id);
        await _suppliers.DeleteOneAsync(filter);
    }

    public async Task<Supplier?> GetByIdAsync(ObjectId id)
    {
        return await _suppliers.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Supplier>> GetAllAsync()
    {
        return await _suppliers.Find(_ => true).ToListAsync();
    }
}
