// src/Services/OrderSaga.Worker/Repositories/SagaStateRepository.cs
using Dapper;
using Microsoft.Data.SqlClient;
using OrderSaga.Worker.Entities;

namespace OrderSaga.Worker.Repositories;

public class SagaStateRepository : ISagaStateRepository
{
    private readonly string _connectionString;

    public SagaStateRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SagaStateDb")
            ?? throw new ArgumentNullException("ConnectionString:SagaStateDb is required");
    }

    public async Task SaveSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = @"
            INSERT INTO OrderSagaStates (
                Id, OrderId, UserId, TotalAmount, Status, CurrentStep, 
                PaymentTransactionId, StartedAt, FailureReason
            ) VALUES (
                @Id, @OrderId, @UserId, @TotalAmount, @Status, @CurrentStep, 
                @PaymentTransactionId, @StartedAt, @FailureReason
            )";

        await connection.ExecuteAsync(sql, state);
    }

    public async Task UpdateSagaStateAsync(OrderSagaState state, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = @"
            UPDATE OrderSagaStates 
            SET Status = @Status, 
                CurrentStep = @CurrentStep,
                PaymentTransactionId = @PaymentTransactionId,
                CompletedAt = @CompletedAt,
                FailureReason = @FailureReason,
                UpdatedAt = GETDATE()
            WHERE Id = @Id";

        await connection.ExecuteAsync(sql, state);
    }

    public async Task<OrderSagaState> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = "SELECT * FROM OrderSagaStates WHERE OrderId = @OrderId";

        return await connection.QueryFirstOrDefaultAsync<OrderSagaState>(sql, new { OrderId = orderId });
    }

    public async Task<IEnumerable<OrderSagaState>> GetPendingSagasAsync(CancellationToken cancellationToken = default)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        const string sql = @"
            SELECT * FROM OrderSagaStates 
            WHERE Status IN (0, 2, 3) -- Started, Failed, CompensationFailed
            AND StartedAt > DATEADD(HOUR, -24, GETDATE())";


        return await connection.QueryAsync<OrderSagaState>(sql);
    }
}
