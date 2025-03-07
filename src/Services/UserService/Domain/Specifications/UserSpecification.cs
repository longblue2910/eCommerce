using Domain.Entities;
using SharedKernel.Common;

namespace Domain.Specifications;

public class UserSpecification : Specification<User>
{
    public UserSpecification(
        string username,
        string email,
        string fullName,
        string phoneNumber,
        bool? isActive,
        DateTime? createdFrom,
        DateTime? createdTo
    )
    {
        Criteria = user =>
            (string.IsNullOrWhiteSpace(username) || user.Username.Contains(username)) &&
            (string.IsNullOrWhiteSpace(email) || user.Email.Contains(email)) &&
            (string.IsNullOrWhiteSpace(fullName) || user.FullName.Contains(fullName)) &&
            (string.IsNullOrWhiteSpace(phoneNumber) || user.PhoneNumber.Contains(phoneNumber)) &&
            (!isActive.HasValue || user.IsActive == isActive.Value) &&
            (!createdFrom.HasValue || user.CreatedAt >= createdFrom.Value) &&
            (!createdTo.HasValue || user.CreatedAt <= createdTo.Value);
    }
}
