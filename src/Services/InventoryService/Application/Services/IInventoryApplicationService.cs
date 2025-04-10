// src/Services/InventoryService/Application/Services/IInventoryApplicationService.cs
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Services;

/// <summary>
/// Service xử lý nghiệp vụ tồn kho cấp ứng dụng
/// </summary>
public interface IInventoryApplicationService
{
    /// <summary>
    /// Kiểm tra tồn kho cho danh sách sản phẩm
    /// </summary>
    Task<List<ProductInventoryStatusDto>> CheckInventoryAsync(
        List<ProductQuantityDto> products,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Giữ hàng trong kho cho đơn hàng
    /// </summary>
    Task<List<ProductReservationResultDto>> ReserveInventoryAsync(
        Guid orderId,
        List<ProductQuantityDto> products,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Giải phóng hàng trong kho khi hủy đơn hàng
    /// </summary>
    Task<bool> ReleaseInventoryAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Xác nhận đã sử dụng hàng trong kho khi đơn hàng hoàn tất
    /// </summary>
    Task<bool> ConfirmInventoryAsync(
        Guid orderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Đăng ký nhận thông báo thay đổi tồn kho
    /// </summary>
    Task<IInventoryChangeSubscription> SubscribeToInventoryChangesAsync(
        List<Guid> productIds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Hủy đăng ký nhận thông báo thay đổi tồn kho
    /// </summary>
    Task UnsubscribeFromInventoryChangesAsync(
        Guid subscriptionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy lịch sử thay đổi tồn kho của sản phẩm
    /// </summary>
    Task<List<InventoryHistoryDto>> GetInventoryHistoryAsync(
        Guid productId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default);
}
