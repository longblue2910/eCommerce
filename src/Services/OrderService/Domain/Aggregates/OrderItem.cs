namespace Domain.Aggregates;

using SharedKernel.Common;
using System;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; } // ID của đơn hàng chứa sản phẩm này
    public Guid ProductId { get; private set; } // ID của sản phẩm
    public int Quantity { get; private set; } // Số lượng sản phẩm
    public decimal Price { get; private set; } // Giá sản phẩm tại thời điểm mua
    public decimal TotalPrice => Quantity * Price; // Tổng giá trị của sản phẩm này trong đơn hàng

    private OrderItem() : base(Guid.NewGuid()) { }

    public OrderItem(Guid orderId, Guid productId, int quantity, decimal price) : base(Guid.NewGuid())
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        Price = price;
    }
}
