// src/Services/OrderSaga.Worker/Services/Implementations/OrderServiceClient.cs
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace OrderSaga.Worker.Services.Implementations;

public class OrderServiceClient : IOrderServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderServiceClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> MarkOrderAsProcessingAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/api/orders/{orderId}/processing", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking order {OrderId} as processing", orderId);
            return false;
        }
    }

    public async Task<bool> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PutAsync($"/api/orders/{orderId}/paid", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking order {OrderId} as paid", orderId);
            return false;
        }
    }

    public async Task<bool> MarkOrderAsCancelledAsync(Guid orderId, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { reason }),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"/api/orders/{orderId}/cancelled", content, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking order {OrderId} as cancelled", orderId);
            return false;
        }
    }

    public async Task<OrderDto?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<OrderDto>($"/api/orders/{orderId}", _jsonOptions, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting order {OrderId}", orderId);
            return null;
        }
    }
}

public interface IOrderServiceClient
{
    Task<bool> MarkOrderAsProcessingAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<bool> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<bool> MarkOrderAsCancelledAsync(Guid orderId, string reason, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);

}

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItem> Items { get; set; }

}


// Model cho dữ liệu trả về
public class OrderDetails
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; }
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

