using Application.Users.Dtos;
using AutoMapper;
using Domain.Interfaces.Repositories;
using MediatR;
using SharedKernel.Exceptions;

namespace Application.Queries.User;

public record GetUserByIdQuery(Guid UserId) : IRequest<UserResponse>;

public class GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    : IRequestHandler<GetUserByIdQuery, UserResponse>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<UserResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException($"User id {request.UserId} not found.");

        return _mapper.Map<UserResponse>(user);
    }
}
