using MediatR;
using SharedKernel.Common;

namespace Domain.Events.User;

/// <summary>
/// Khi user đăng ký
/// </summary>
/// <param name="userId"></param>
/// <param name="username"></param>
/// <param name="email"></param>
public class UserRegisteredEvent(Guid userId, string username, string email) : IDomainEvent, INotification
{
    public Guid UserId { get; } = userId;
    public string Username { get; } = username;
    public string Email { get; } = email;
    public DateTime OccurredOn { get; } = DateTime.Now;
}

/*
 * 📌 Khi nào event này xảy ra?
 * Khi user đăng ký tài khoản mới, sự kiện này sẽ được phát đi.
 * Hệ thống email service có thể lắng nghe và gửi email chào mừng.
 */

/*
 * 📌 📂 Ai sẽ lắng nghe?
public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly IEmailService _emailService;

    public UserRegisteredEventHandler(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        await _emailService.SendWelcomeEmailAsync(notification.Email, notification.Username);
    }
}

✔️ Kết quả: Gửi email chào mừng user.

 */