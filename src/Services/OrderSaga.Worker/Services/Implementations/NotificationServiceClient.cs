// src/Services/OrderSaga.Worker/Services/Implementations/NotificationServiceClient.cs
using System.Net.Http.Json;

namespace OrderSaga.Worker.Services.Implementations;

public class NotificationServiceClient : INotificationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationServiceClient> _logger;

    public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendOrderConfirmationAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/notifications/order-confirmation", new { orderId, userId }, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order confirmation for order {OrderId} to user {UserId}", orderId, userId);
        }
    }

    public async Task SendOrderCancellationAsync(Guid orderId, Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/notifications/order-cancellation", new { orderId, userId, reason }, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order cancellation for order {OrderId} to user {UserId}, reason: {Reason}", orderId, userId, reason);
        }
    }

    public async Task SendOrderStatusUpdateAsync(Guid orderId, Guid userId, string status, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/notifications/order-status-update", new { orderId, userId, status }, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending order status update for order {OrderId} to user {UserId}, status: {Status}", orderId, userId, status);
        }
    }
}
