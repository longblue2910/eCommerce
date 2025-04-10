// src/Services/InventoryService/API/GrpcServices/InventoryGrpcService.cs
using Grpc.Core;
using InventoryService.Application.DTOs;
using InventoryService.Application.Services;
using InventoryService.Protos;

namespace InventoryService.API.GrpcServices;

public class InventoryGrpcService : InventoryGrpc.InventoryGrpcBase
{
    private readonly IInventoryApplicationService _inventoryService;
    private readonly ILogger<InventoryGrpcService> _logger;

    public InventoryGrpcService(IInventoryApplicationService inventoryService, ILogger<InventoryGrpcService> logger)
    {
        _inventoryService = inventoryService ?? throw new ArgumentNullException(nameof(inventoryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<CheckInventoryResponse> CheckInventory(CheckInventoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC CheckInventory called for {Count} products", request.Products.Count);

        try
        {
            var productQuantities = request.Products.Select(p => new ProductQuantityDto
            {
                ProductId = Guid.Parse(p.ProductId),
                Quantity = p.Quantity
            }).ToList();

            var results = await _inventoryService.CheckInventoryAsync(productQuantities, context.CancellationToken);

            var response = new CheckInventoryResponse
            {
                Success = results.All(r => r.InStock),
                Message = results.All(r => r.InStock) ? "All products in stock" : "Some products are out of stock"
            };

            foreach (var result in results)
            {
                response.Products.Add(new ProductInventoryStatus
                {
                    ProductId = result.ProductId.ToString(),
                    InStock = result.InStock,
                    AvailableQuantity = result.AvailableQuantity,
                    Sku = result.Sku,
                    Name = result.Name
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC CheckInventory");
            return new CheckInventoryResponse
            {
                Success = false,
                Message = $"Error checking inventory: {ex.Message}"
            };
        }
    }

    public override async Task<ReserveInventoryResponse> ReserveInventory(ReserveInventoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC ReserveInventory called for order {OrderId}", request.OrderId);

        try
        {
            var orderId = Guid.Parse(request.OrderId);
            var productQuantities = request.Products.Select(p => new ProductQuantityDto
            {
                ProductId = Guid.Parse(p.ProductId),
                Quantity = p.Quantity
            }).ToList();

            var results = await _inventoryService.ReserveInventoryAsync(orderId, productQuantities, context.CancellationToken);

            var response = new ReserveInventoryResponse
            {
                Success = results.All(r => r.Reserved),
                Message = results.All(r => r.Reserved) ? "All products reserved successfully" : "Some products could not be reserved"
            };

            foreach (var result in results)
            {
                response.Products.Add(new ProductReservationStatus
                {
                    ProductId = result.ProductId.ToString(),
                    Reserved = result.Reserved,
                    Message = result.Message
                });
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC ReserveInventory for order {OrderId}", request.OrderId);
            return new ReserveInventoryResponse
            {
                Success = false,
                Message = $"Error reserving inventory: {ex.Message}"
            };
        }
    }

    public override async Task<ReleaseInventoryResponse> ReleaseInventory(ReleaseInventoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC ReleaseInventory called for order {OrderId}", request.OrderId);

        try
        {
            var orderId = Guid.Parse(request.OrderId);
            var success = await _inventoryService.ReleaseInventoryAsync(orderId, context.CancellationToken);

            return new ReleaseInventoryResponse
            {
                Success = success,
                Message = success ? "Inventory released successfully" : "Failed to release inventory"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC ReleaseInventory for order {OrderId}", request.OrderId);
            return new ReleaseInventoryResponse
            {
                Success = false,
                Message = $"Error releasing inventory: {ex.Message}"
            };
        }
    }

    public override async Task<ConfirmInventoryResponse> ConfirmInventory(ConfirmInventoryRequest request, ServerCallContext context)
    {
        _logger.LogInformation("gRPC ConfirmInventory called for order {OrderId}", request.OrderId);

        try
        {
            var orderId = Guid.Parse(request.OrderId);
            var success = await _inventoryService.ConfirmInventoryAsync(orderId, context.CancellationToken);

            return new ConfirmInventoryResponse
            {
                Success = success,
                Message = success ? "Inventory confirmed successfully" : "Failed to confirm inventory"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in gRPC ConfirmInventory for order {OrderId}", request.OrderId);
            return new ConfirmInventoryResponse
            {
                Success = false,
                Message = $"Error confirming inventory: {ex.Message}"
            };
        }
    }
}
