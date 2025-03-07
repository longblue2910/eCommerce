using Domain.Aggregates;
using Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoCategoryRepository(MongoDbContext context) : ICategoryRepository
{
    private readonly IMongoCollection<Category> _categories = context.GetCollection<Category>("Categories");

    public async Task AddAsync(Category category)
    {
        await _categories.InsertOneAsync(category);
    }

    public async Task UpdateAsync(Category category)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.Id, category.Id);
        await _categories.ReplaceOneAsync(filter, category);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        var filter = Builders<Category>.Filter.Eq(c => c.Id, id);
        await _categories.DeleteOneAsync(filter);
    }

    public async Task<Category?> GetByIdAsync(ObjectId id)
    {
        return await _categories.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _categories.Find(_ => true).ToListAsync();
    }

    public async Task<List<Category>> GetSubCategoriesAsync(ObjectId parentId)
    {
        return await _categories.Find(c => c.ParentId == parentId).ToListAsync();
    }

    public async Task<bool> IsNameExistsAsync(string name)
    {
        var count = await _categories.CountDocumentsAsync(c => c.Name == name);
        return count > 0;
    }
}
