using Domain.Aggregates;
using Domain.Interfaces.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoProductRepository(MongoDbContext context) : IProductRepository
{
    private readonly IMongoCollection<Product> _products = context.GetCollection<Product>("Products");

    public async Task AddAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, product.Id);
        await _products.ReplaceOneAsync(filter, product);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
        await _products.DeleteOneAsync(filter);
    }

    public async Task<Product?> GetByIdAsync(ObjectId id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetByCategoryIdAsync(ObjectId categoryId)
    {
        return await _products.Find(p => p.CategoryId == categoryId).ToListAsync();
    }

    public async Task<List<Product>> GetByBrandIdAsync(ObjectId brandId)
    {
        return await _products.Find(p => p.BrandId == brandId).ToListAsync();
    }

    public async Task<List<Product>> SearchByNameAsync(string keyword)
    {
        var filter = Builders<Product>.Filter.Regex(p => p.Name, new BsonRegularExpression(keyword, "i"));
        return await _products.Find(filter).ToListAsync();
    }

    public async Task<List<Product>> GetAvailableProductsAsync()
    {
        var filter = Builders<Product>.Filter.Gt(p => p.StockQuantity, 0);
        return await _products.Find(filter).ToListAsync();
    }

    public async Task<bool> IsSkuExistsAsync(string sku)
    {
        var count = await _products.CountDocumentsAsync(p => p.SKU == sku);
        return count > 0;
    }
}
