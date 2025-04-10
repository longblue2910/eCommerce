namespace InventoryService.Domain.Entities;

public class InventoryHistory
{
    public Guid Id { get; private set; }
    public Guid InventoryId { get; private set; }
    public Guid? OrderId { get; private set; }
    public string EventType { get; private set; } // RESERVED, RELEASED, CONFIRMED, ADJUSTED
    public int QuantityChanged { get; private set; }
    public int NewTotalQuantity { get; private set; }
    public int NewReservedQuantity { get; private set; }
    public DateTime Timestamp { get; private set; }

    private InventoryHistory() { }

    public InventoryHistory(Guid inventoryId, Guid? orderId, string eventType,
        int quantityChanged, int newTotalQuantity, int newReservedQuantity)
    {
        Id = Guid.NewGuid();
        InventoryId = inventoryId;
        OrderId = orderId;
        EventType = eventType;
        QuantityChanged = quantityChanged;
        NewTotalQuantity = newTotalQuantity;
        NewReservedQuantity = newReservedQuantity;
        Timestamp = DateTime.UtcNow;
    }
}
