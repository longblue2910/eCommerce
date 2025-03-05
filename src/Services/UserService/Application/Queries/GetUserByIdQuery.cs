using Application.DTOs;
using MediatR;

namespace Application.Queries;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserDto>;

public class GetUserByIdQueryHandler: IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
