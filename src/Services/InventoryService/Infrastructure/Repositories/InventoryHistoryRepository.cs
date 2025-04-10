// src/Services/InventoryService/Infrastructure/Repositories/InventoryHistoryRepository.cs
using InventoryService.Domain.Entities;
using InventoryService.Domain.Repositories;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.Repositories;

public class InventoryHistoryRepository : IInventoryHistoryRepository
{
    private readonly InventoryDbContext _dbContext;
    private readonly ILogger<InventoryHistoryRepository> _logger;

    public InventoryHistoryRepository(InventoryDbContext dbContext, ILogger<InventoryHistoryRepository> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<InventoryHistory>> GetByInventoryIdAsync(
        Guid inventoryId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InventoryHistories
            .Where(h => h.InventoryId == inventoryId && h.Timestamp >= startDate && h.Timestamp <= endDate)
            .OrderByDescending(h => h.Timestamp)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(InventoryHistory history, CancellationToken cancellationToken = default)
    {
        await _dbContext.InventoryHistories.AddAsync(history, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Created inventory history for inventory {InventoryId}, event type {EventType}",
            history.InventoryId, history.EventType);
    }
}
