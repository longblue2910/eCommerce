namespace OrderSaga.Worker.Services;

/// <summary>
/// Client để tương tác với Notification Service
/// </summary>
public interface INotificationServiceClient
{
    /// <summary>
    /// Gửi thông báo xác nhận đơn hàng
    /// </summary>
    Task SendOrderConfirmationAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gửi thông báo hủy đơn hàng
    /// </summary>
    Task SendOrderCancellationAsync(Guid orderId, Guid userId, string reason, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gửi thông báo cập nhật trạng thái đơn hàng
    /// </summary>
    Task SendOrderStatusUpdateAsync(Guid orderId, Guid userId, string status, CancellationToken cancellationToken = default);
}
