using Domain.Aggregates;
using MongoDB.Bson;

namespace Domain.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(ObjectId id);
    Task<Category?> GetByIdAsync(ObjectId id);
    Task<List<Category>> GetAllAsync();
    Task<List<Category>> GetSubCategoriesAsync(ObjectId parentId);
    Task<bool> IsNameExistsAsync(string name);
}
