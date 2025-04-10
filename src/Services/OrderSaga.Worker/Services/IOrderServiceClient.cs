//// src/Services/OrderSaga.Worker/Services/IOrderServiceClient.cs
//public interface IOrderServiceClient
//{
//    // Các phương thức hiện có

//    /// <summary>
//    /// Lấy chi tiết đơn hàng bao gồm danh sách sản phẩm
//    /// </summary>
//    Task<OrderDetails> GetOrderDetailsAsync(Guid orderId, CancellationToken cancellationToken = default);
//}

//// Model cho dữ liệu trả về
//public class OrderDetails
//{
//    public Guid OrderId { get; set; }
//    public Guid UserId { get; set; }
//    public decimal TotalAmount { get; set; }
//    public List<OrderItem> Items { get; set; }
//}

//public class OrderItem
//{
//    public Guid ProductId { get; set; }
//    public int Quantity { get; set; }
//    public decimal Price { get; set; }
//}
