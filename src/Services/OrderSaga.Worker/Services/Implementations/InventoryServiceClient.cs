// src/Services/OrderSaga.Worker/Services/Implementations/InventoryServiceClient.cs
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace OrderSaga.Worker.Services.Implementations;

public class InventoryServiceClient : IInventoryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryServiceClient> _logger;

    public InventoryServiceClient(HttpClient httpClient, ILogger<InventoryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> ReserveInventoryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/inventory/reserve/{orderId}", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reserving inventory for order {OrderId}", orderId);
            return false;
        }
    }

    public async Task<bool> ReleaseInventoryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/inventory/release/{orderId}", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing inventory for order {OrderId}", orderId);
            return false;
        }
    }

    public async Task<bool> ConfirmInventoryAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync($"/api/inventory/confirm/{orderId}", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming inventory for order {OrderId}", orderId);
            return false;
        }
    }
}
