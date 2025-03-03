using SharedKernel.Common;

namespace Domain.Events.User;

/// <summary>
/// Khi user bị vô hiệu hóa
/// </summary>
/// <param name="userId"></param>
public class UserDeactivatedEvent(Guid userId) : IDomainEvent
{
    public Guid UserId { get; } = userId;
    public DateTime OccurredOn { get; } = DateTime.Now;
}

/*
 * 
 * 📌 Khi nào event này xảy ra?
   Khi admin hoặc user vô hiệu hóa tài khoản, sự kiện sẽ được phát đi.
   Các service khác có thể chặn truy cập user này.

   📌 📂 Ai sẽ lắng nghe?
   public class UserDeactivatedEventHandler : INotificationHandler<UserDeactivatedEvent>
   {
       private readonly IAuthService _authService;
   
       public UserDeactivatedEventHandler(IAuthService authService)
       {
           _authService = authService;
       }
   
       public async Task Handle(UserDeactivatedEvent notification, CancellationToken cancellationToken)
       {
           await _authService.RevokeUserSessionsAsync(notification.UserId);
       }
   }
   ✔️ Kết quả: Logout user ngay lập tức.

 */