using Domain.Aggregates;
using Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoReviewRepository(MongoDbContext context) : IReviewRepository
{
    private readonly IMongoCollection<Review> _reviews = context.GetCollection<Review>("Reviews");

    public async Task AddAsync(Review review)
    {
        await _reviews.InsertOneAsync(review);
    }

    public async Task<List<Review>> GetByCustomerIdAsync(ObjectId customerId)
    {
        return await _reviews.Find(r => r.CustomerId == customerId).ToListAsync();
    }

    public async Task<List<Review>> GetByProductIdAsync(ObjectId productId)
    {
        return await _reviews.Find(r => r.ProductId == productId).ToListAsync();
    }
}
