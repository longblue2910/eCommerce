//using OrderSaga.Worker.Entities;

//namespace OrderSaga.Worker.Services;

///// <summary>
///// Client để tương tác với Order Service
///// </summary>
//public interface IOrderServiceClient
//{
//    Task<bool> MarkOrderAsProcessingAsync(Guid orderId, CancellationToken cancellationToken = default);
//    Task<bool> MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default);
//    Task<bool> MarkOrderAsCancelledAsync(Guid orderId, string reason, CancellationToken cancellationToken = default);
//    Task<OrderDto?> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
//}