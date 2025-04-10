// src/Services/InventoryService/Domain/Repositories/IInventoryRepository.cs
namespace InventoryService.Domain.Repositories;

using InventoryService.Domain.Entities;

public interface IInventoryRepository
{
    Task<Inventory> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Inventory> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<List<Inventory>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<List<Inventory>> GetByProductIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
    Task CreateAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task UpdateAsync(Inventory inventory, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
