using SharedKernel.Common;

namespace Domain.Events.User;

/// <summary>
/// User được gán quyền
/// </summary>
public class UserRoleAssignedEvent(Guid userId, string role) : IDomainEvent
{
    public Guid UserId { get; } = userId;
    public string Role { get; } = role;
    public DateTime OccurredOn { get; } = DateTime.Now;
}
/*
 * 📌 Khi nào event này xảy ra?
   Khi user được cấp quyền mới (Admin, Manager, Customer).
   Hệ thống quản lý quyền có thể cập nhật user này.

   📌 📂 Ai sẽ lắng nghe?
   public class UserRoleAssignedEventHandler : INotificationHandler<UserRoleAssignedEvent>
   {
       private readonly IPermissionService _permissionService;
   
       public UserRoleAssignedEventHandler(IPermissionService permissionService)
       {
           _permissionService = permissionService;
       }
   
       public async Task Handle(UserRoleAssignedEvent notification, CancellationToken cancellationToken)
       {
           await _permissionService.UpdateUserPermissionsAsync(notification.UserId, notification.Role);
       }
   }
 */