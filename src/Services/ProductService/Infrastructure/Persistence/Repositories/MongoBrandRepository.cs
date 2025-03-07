using Domain.Aggregates;
using Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoBrandRepository(MongoDbContext context) : IBrandRepository
{
    private readonly IMongoCollection<Brand> _brands = context.GetCollection<Brand>("Brands");

    public async Task AddAsync(Brand brand)
    {
        await _brands.InsertOneAsync(brand);
    }

    public async Task UpdateAsync(Brand brand)
    {
        var filter = Builders<Brand>.Filter.Eq(b => b.Id, brand.Id);
        await _brands.ReplaceOneAsync(filter, brand);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        var filter = Builders<Brand>.Filter.Eq(b => b.Id, id);
        await _brands.DeleteOneAsync(filter);
    }

    public async Task<Brand?> GetByIdAsync(ObjectId id)
    {
        return await _brands.Find(b => b.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        return await _brands.Find(_ => true).ToListAsync();
    }

    public async Task<bool> IsNameExistsAsync(string name)
    {
        var count = await _brands.CountDocumentsAsync(b => b.Name == name);
        return count > 0;
    }
}
