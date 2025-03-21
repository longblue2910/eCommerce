using Domain.Aggregates;
using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

public static class ProductDbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        // ✅ Sử dụng ILoggerFactory để tạo logger
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("ProductDbSeeder");

        var mongoSettings = scope.ServiceProvider.GetRequiredService<MongoSettings>();

        try
        {

            var client = new MongoClient(mongoSettings.ConnectionString);
            var database = client.GetDatabase(mongoSettings.DatabaseName);

            var categoryCollection = database.GetCollection<Category>("Categories");
            var brandCollection = database.GetCollection<Brand>("Brands");
            var productCollection = database.GetCollection<Product>("Products");

            // Kiểm tra xem có dữ liệu chưa, nếu chưa thì thêm mới
            if (await categoryCollection.CountDocumentsAsync(FilterDefinition<Category>.Empty) == 0)
            {
                logger.LogInformation("Seeding categories...");

                var categories = new List<Category>
                {
                    new(ObjectId.GenerateNewId(), "Bánh Huế"),
                    new(ObjectId.GenerateNewId(), "Đồ ăn vặt"),
                    new(ObjectId.GenerateNewId(), "Đồ uống")
                };

                await categoryCollection.InsertManyAsync(categories);
                logger.LogInformation("Category seeding completed.");
            }

            if (await brandCollection.CountDocumentsAsync(FilterDefinition<Brand>.Empty) == 0)
            {
                logger.LogInformation("Seeding brands...");

                var brands = new List<Brand>
                {
                    new(ObjectId.GenerateNewId(), "Thương hiệu A"),
                    new(ObjectId.GenerateNewId(), "Thương hiệu B"),
                    new(ObjectId.GenerateNewId(), "Thương hiệu C")
                };

                await brandCollection.InsertManyAsync(brands);
                logger.LogInformation("Brand seeding completed.");
            }

            if (await productCollection.CountDocumentsAsync(FilterDefinition<Product>.Empty) == 0)
            {
                logger.LogInformation("Seeding products...");

                var category = await categoryCollection.Find(FilterDefinition<Category>.Empty).FirstOrDefaultAsync();
                var brand = await brandCollection.Find(FilterDefinition<Brand>.Empty).FirstOrDefaultAsync();

                if (category == null || brand == null)
                {
                    logger.LogWarning("No categories or brands found. Skipping product seeding.");
                    return;
                }

                var products = new List<Product>
                {
                    new(ObjectId.GenerateNewId(), "Bánh lọc", "Bánh lọc Huế truyền thống", 5000, 100, "SKU001", category.Id, brand.Id),
                    new(ObjectId.GenerateNewId(), "Bánh nậm", "Bánh nậm mềm thơm", 7000, 80, "SKU002", category.Id, brand.Id),
                    new(ObjectId.GenerateNewId(), "Bánh ép", "Bánh ép Huế giòn rụm", 10000, 50, "SKU003", category.Id, brand.Id)
                };

                await productCollection.InsertManyAsync(products);
                logger.LogInformation("Product seeding completed.");
            }
            else
            {
                logger.LogInformation("Products already exist, skipping seeding.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }
}
