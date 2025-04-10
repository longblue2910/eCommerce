// src/Services/OrderSaga.Worker/Services/Implementations/InventoryServiceClient.cs
using Grpc.Net.Client;
using InventoryService.Protos;

namespace OrderSaga.Worker.Services.Implementations;

/// <summary>
/// Client sử dụng gRPC để tương tác với Inventory Service
/// </summary>
public class InventoryServiceClient : IInventoryServiceClient
{
    private readonly ILogger<InventoryServiceClient> _logger;
    private readonly InventoryGrpc.InventoryGrpcClient _grpcClient;
    private readonly IOrderServiceClient _orderServiceClient;

    public InventoryServiceClient(
        ILogger<InventoryServiceClient> logger,
        IConfiguration configuration,
        IOrderServiceClient orderServiceClient)
    {
        _logger = logger;
        _orderServiceClient = orderServiceClient;

        // Tạo kết nối gRPC
        var channel = GrpcChannel.ForAddress(configuration["ServiceUrls:InventoryService"]);
        _grpcClient = new InventoryGrpc.InventoryGrpcClient(channel);
    }

    /// <summary>
    /// Giữ hàng trong kho cho đơn hàng
    /// </summary>
    public async Task<bool> ReserveInventoryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Đang giữ hàng trong kho cho đơn hàng {OrderId} thông qua gRPC", orderId);

            // Lấy thông tin chi tiết đơn hàng (sản phẩm + số lượng) từ Order Service
            var orderDetails = await _orderServiceClient.GetOrderAsync(orderId, cancellationToken);

            if (orderDetails == null || orderDetails.Items == null || orderDetails.Items.Count == 0)
            {
                _logger.LogWarning("Không thể lấy thông tin đơn hàng hoặc đơn hàng không có sản phẩm");
                return false;
            }

            // Tạo request gRPC
            var request = new ReserveInventoryRequest
            {
                OrderId = orderId.ToString()
            };

            // Thêm các sản phẩm vào request
            foreach (var item in orderDetails.Items)
            {
                request.Products.Add(new ProductQuantity
                {
                    ProductId = item.ProductId.ToString(),
                    Quantity = item.Quantity
                });
            }

            // Gọi service gRPC
            var response = await _grpcClient.ReserveInventoryAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(30), // timeout 30 giây
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("Giữ hàng thành công cho đơn hàng {OrderId}", orderId);
            }
            else
            {
                _logger.LogWarning("Giữ hàng thất bại cho đơn hàng {OrderId}: {Message}",
                    orderId, response.Message);

                // Log chi tiết sản phẩm không sẵn có
                foreach (var product in response.Products)
                {
                    if (!product.Reserved)
                    {
                        _logger.LogWarning("Sản phẩm {ProductId} không thể giữ hàng: {Message}",
                            product.ProductId, product.Message);
                    }
                }
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi ReserveInventory qua gRPC cho đơn hàng {OrderId}", orderId);
            return false;
        }
    }

    /// <summary>
    /// Giải phóng hàng trong kho khi hủy đơn hàng
    /// </summary>
    public async Task<bool> ReleaseInventoryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Đang giải phóng hàng trong kho cho đơn hàng {OrderId} thông qua gRPC", orderId);

            var request = new ReleaseInventoryRequest
            {
                OrderId = orderId.ToString()
            };

            var response = await _grpcClient.ReleaseInventoryAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(30),
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("Giải phóng hàng thành công cho đơn hàng {OrderId}", orderId);
            }
            else
            {
                _logger.LogWarning("Giải phóng hàng thất bại cho đơn hàng {OrderId}: {Message}",
                    orderId, response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi ReleaseInventory qua gRPC cho đơn hàng {OrderId}", orderId);
            return false;
        }
    }

    /// <summary>
    /// Xác nhận đã sử dụng hàng trong kho khi đơn hàng hoàn tất
    /// </summary>
    public async Task<bool> ConfirmInventoryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Đang xác nhận sử dụng hàng trong kho cho đơn hàng {OrderId} thông qua gRPC", orderId);

            var request = new ConfirmInventoryRequest
            {
                OrderId = orderId.ToString()
            };

            var response = await _grpcClient.ConfirmInventoryAsync(
                request,
                deadline: DateTime.UtcNow.AddSeconds(30),
                cancellationToken: cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("Xác nhận sử dụng hàng thành công cho đơn hàng {OrderId}", orderId);
            }
            else
            {
                _logger.LogWarning("Xác nhận sử dụng hàng thất bại cho đơn hàng {OrderId}: {Message}",
                    orderId, response.Message);
            }

            return response.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gọi ConfirmInventory qua gRPC cho đơn hàng {OrderId}", orderId);
            return false;
        }
    }
}
