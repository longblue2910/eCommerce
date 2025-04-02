using OrderSaga.Worker.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderSaga.Worker.Repositories;


/// <summary>
/// Repository để lưu trữ và truy xuất trạng thái Saga
/// </summary>
public interface ISagaStateRepository
{
    /// <summary>
    /// Lưu trạng thái Saga mới
    /// </summary>
    Task SaveSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cập nhật trạng thái Saga hiện có
    /// </summary>
    Task UpdateSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy trạng thái Saga theo ID của đơn hàng
    /// </summary>
    Task<OrderSagaState?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lấy tất cả các saga đang chờ xử lý hoặc bị lỗi
    /// </summary>
    Task<IEnumerable<OrderSagaState>> GetPendingSagasAsync(CancellationToken cancellationToken = default);
}
