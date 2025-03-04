using SharedKernel.Common;

namespace Domain.Events.User;

/// <summary>
/// Khi user đổi mật khẩu
/// </summary>
/// <param name="userId"></param>
public class UserPasswordChangedEvent(Guid userId) : IDomainEvent
{
    public Guid UserId { get; } = userId;
    public DateTime OccurredOn { get; } = DateTime.Now;
}

/*
 * 
  📌 Khi nào event này xảy ra?
  Khi user đổi mật khẩu, hệ thống sẽ phát sự kiện này.
  Các hệ thống liên quan (như bảo mật, logging) có thể lắng nghe.

  📌 📂 Ai sẽ lắng nghe?
  public class UserPasswordChangedEventHandler : INotificationHandler<UserPasswordChangedEvent>
  {
      private readonly ISecurityService _securityService;
  
      public UserPasswordChangedEventHandler(ISecurityService securityService)
      {
          _securityService = securityService;
      }
  
      public async Task Handle(UserPasswordChangedEvent notification, CancellationToken cancellationToken)
      {
          // Cập nhật lịch sử bảo mật
          await _securityService.LogPasswordChangeAsync(notification.UserId, notification.ChangedAt);
      }
  }
  ✔️ Kết quả: Ghi log bảo mật.

 */