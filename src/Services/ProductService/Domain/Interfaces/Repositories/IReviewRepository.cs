using Domain.Aggregates;
using MongoDB.Bson;

namespace Domain.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task<List<Review>> GetByCustomerIdAsync(ObjectId customerId);
    Task<List<Review>> GetByProductIdAsync(ObjectId productId);
}
