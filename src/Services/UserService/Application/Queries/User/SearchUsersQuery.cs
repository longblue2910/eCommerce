using Application.Users.Dtos;
using Domain.Interfaces.Repositories;
using Domain.Specifications;
using MediatR;
using SharedKernel.Common;

namespace Application.Queries.User;

public class SearchUsersQuery : IRequest<PagedResult<UserResponse>>
{
    public string Username { get; }
    public string Email { get; }
    public string FullName { get; }
    public string PhoneNumber { get; }
    public bool? IsActive { get; }
    public DateTime? CreatedFrom { get; }
    public DateTime? CreatedTo { get; }
    public int Page { get; }
    public int PageSize { get; }

    public SearchUsersQuery(
        string username, string email, string fullName, string phoneNumber,
        bool? isActive, DateTime? createdFrom, DateTime? createdTo, int page, int pageSize)
    {
        Username = username;
        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        IsActive = isActive;
        CreatedFrom = createdFrom;
        CreatedTo = createdTo;
        Page = page;
        PageSize = pageSize;
    }
}

public class SearchUsersQueryHandler(IUserRepository userRepository) : IRequestHandler<SearchUsersQuery, PagedResult<UserResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<PagedResult<UserResponse>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var specification = new UserSpecification(
            request.Username, request.Email, request.FullName, request.PhoneNumber,
            request.IsActive, request.CreatedFrom, request.CreatedTo
        );

        var pagedUsers = await _userRepository.GetUsersAsync(specification, request.Page, request.PageSize);

        // 🔥 Convert User -> UserResponse
        var userResponses = pagedUsers.Items.Select(user => new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            Roles = [.. user.Roles.Select(r => r.Name)]
        }).ToList();

        return new PagedResult<UserResponse>(userResponses, pagedUsers.TotalCount, request.Page, request.PageSize);
    }
}
