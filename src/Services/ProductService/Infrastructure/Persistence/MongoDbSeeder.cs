using Domain.Aggregates;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence;

public class MongoDbSeeder(MongoDbContext context)
{
    private readonly MongoDbContext _context = context;

    public async Task SeedAsync()
    {
        await SeedCategories();
        await SeedBrands();
    }

    private async Task SeedCategories()
    {
        var collection = _context.GetCollection<Category>("Categories");

        bool hasData = await collection.Find(_ => true).AnyAsync();
        if (!hasData)
        {
            var categories = new List<Category>
            {
                new(ObjectId.GenerateNewId(), "Electronics"),
                new(ObjectId.GenerateNewId(), "Clothing"),
                new(ObjectId.GenerateNewId(), "Home Appliances"),
            };

            await collection.InsertManyAsync(categories);
            Console.WriteLine("✅ Seeded Categories");
        }
    }

    private async Task SeedBrands()
    {
        var collection = _context.GetCollection<Brand>("Brands");

        bool hasData = await collection.Find(_ => true).AnyAsync();
        if (!hasData)
        {
            var brands = new List<Brand>
            {
                new(ObjectId.GenerateNewId(), "Apple"),
                new(ObjectId.GenerateNewId(), "Samsung"),
                new(ObjectId.GenerateNewId(), "Nike"),
            };

            await collection.InsertManyAsync(brands);
            Console.WriteLine("✅ Seeded Brands");
        }
    }
}
