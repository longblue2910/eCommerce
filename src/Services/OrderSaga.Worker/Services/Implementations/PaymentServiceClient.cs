// src/Services/OrderSaga.Worker/Services/Implementations/PaymentServiceClient.cs
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace OrderSaga.Worker.Services.Implementations;

public class PaymentServiceClient : IPaymentServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PaymentServiceClient> _logger;

    public PaymentServiceClient(HttpClient httpClient, ILogger<PaymentServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(Guid orderId, Guid userId, decimal amount, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/payments/process", new { orderId, userId, amount }, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<PaymentResult>(cancellationToken: cancellationToken) ?? new PaymentResult { IsSuccessful = false };
            }
            return new PaymentResult { IsSuccessful = false, FailureReason = response.ReasonPhrase };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment for order {OrderId}", orderId);
            return new PaymentResult { IsSuccessful = false, FailureReason = ex.Message };
        }
    }

    public async Task<bool> RefundPaymentAsync(Guid orderId, string transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/payments/refund", new { orderId, transactionId }, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refunding payment for order {OrderId}, transaction {TransactionId}", orderId, transactionId);
            return false;
        }
    }
}
