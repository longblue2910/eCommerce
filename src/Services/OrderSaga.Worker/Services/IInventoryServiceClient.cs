namespace OrderSaga.Worker.Services;

/// <summary>
/// Client để tương tác với Inventory Service
/// </summary>
public interface IInventoryServiceClient
{
    /// <summary>
    /// Giữ hàng trong kho cho đơn hàng
    /// </summary>
    Task<bool> ReserveInventoryAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Giải phóng hàng trong kho khi hủy đơn hàng
    /// </summary>
    Task<bool> ReleaseInventoryAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Xác nhận đã sử dụng hàng trong kho khi đơn hàng hoàn tất
    /// </summary>
    Task<bool> ConfirmInventoryAsync(Guid orderId, CancellationToken cancellationToken = default);
}
