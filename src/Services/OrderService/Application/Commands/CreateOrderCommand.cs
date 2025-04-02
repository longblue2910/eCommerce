namespace Application.Commands;

using Domain.Aggregates;
using MediatR;

public record CreateOrderCommand(Guid UserId, List<OrderItem> Items) : IRequest<Order>;