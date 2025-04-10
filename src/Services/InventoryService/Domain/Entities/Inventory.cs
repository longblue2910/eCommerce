namespace InventoryService.Domain.Entities;

public class Inventory
{
    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Sku { get; private set; }
    public int TotalQuantity { get; private set; }
    public int ReservedQuantity { get; private set; }
    public int AvailableQuantity => TotalQuantity - ReservedQuantity;
    public string LocationCode { get; private set; }
    public DateTime LastUpdated { get; private set; }

    // Tracking dưới dạng dictionary (OrderId -> Quantity)
    // Theo dõi hàng đặt giữ cho từng đơn hàng
    private readonly Dictionary<Guid, int> _reservations;
    public IReadOnlyDictionary<Guid, int> Reservations => _reservations;

    private Inventory()
    {
        _reservations = new Dictionary<Guid, int>();
    }

    public Inventory(Guid productId, string sku, int totalQuantity, string locationCode)
        : this()
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Sku = sku;
        TotalQuantity = totalQuantity;
        LocationCode = locationCode;
        LastUpdated = DateTime.UtcNow;
    }

    public bool CanReserve(int quantity)
    {
        return AvailableQuantity >= quantity;
    }

    public bool Reserve(Guid orderId, int quantity)
    {
        if (!CanReserve(quantity))
            return false;

        if (_reservations.ContainsKey(orderId))
        {
            // Tăng số lượng nếu đã có reservation trước đó
            _reservations[orderId] += quantity;
        }
        else
        {
            _reservations.Add(orderId, quantity);
        }

        ReservedQuantity += quantity;
        LastUpdated = DateTime.UtcNow;
        return true;
    }

    public bool Release(Guid orderId)
    {
        if (!_reservations.TryGetValue(orderId, out int reservedQuantity))
            return false;

        ReservedQuantity -= reservedQuantity;
        _reservations.Remove(orderId);
        LastUpdated = DateTime.UtcNow;
        return true;
    }

    public bool Confirm(Guid orderId)
    {
        if (!_reservations.TryGetValue(orderId, out int reservedQuantity))
            return false;

        ReservedQuantity -= reservedQuantity;
        TotalQuantity -= reservedQuantity;
        _reservations.Remove(orderId);
        LastUpdated = DateTime.UtcNow;
        return true;
    }

    public void AddStock(int quantity)
    {
        TotalQuantity += quantity;
        LastUpdated = DateTime.UtcNow;
    }
}
