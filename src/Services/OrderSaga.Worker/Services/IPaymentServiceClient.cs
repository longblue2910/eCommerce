namespace OrderSaga.Worker.Services;

/// <summary>
/// Client để tương tác với Payment Service
/// </summary>
public interface IPaymentServiceClient
{
    /// <summary>
    /// Xử lý thanh toán cho đơn hàng
    /// </summary>
    Task<PaymentResult> ProcessPaymentAsync(Guid orderId, Guid userId, decimal amount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hoàn tiền đơn hàng khi cần bồi hoàn
    /// </summary>
    Task<bool> RefundPaymentAsync(Guid orderId, string transactionId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Kết quả xử lý thanh toán
/// </summary>
public class PaymentResult
{
    public bool IsSuccessful { get; set; }
    public string TransactionId { get; set; }
    public string FailureReason { get; set; }
}
