using Domain.Events.User;
using MediatR;
using Microsoft.Extensions.Logging;
using SharedKernel.Email;

namespace Application.EventHandlers;

public class UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger, IEmailService emailService) : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger = logger;
    private readonly IEmailService _emailService = emailService;

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🔥 User {Username} (ID: {UserId}) đã đăng ký thành công!", notification.Username, notification.UserId);

        try
        {
            // Gửi email chào mừng
            string subject = "🎉 Chào mừng bạn đến với Rimdasilva!";
            string body = GetWelcomeEmailTemplate(notification.Username);

            await _emailService.SendEmailAsync(notification.Email, subject, body);
            _logger.LogInformation("📧 Email chào mừng đã gửi đến {Email}", notification.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Lỗi khi gửi email chào mừng cho {Email}", notification.Email);
            // Không throw exception để tránh ảnh hưởng đến command chính
        }
    }

    private string GetWelcomeEmailTemplate(string username)
    {
        return $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; text-align: center;'>
                <h2 style='color: #007bff;'>🎉 Chào mừng {username}!</h2>
                <p>Chúng tôi rất vui khi bạn tham gia cùng chúng tôi.</p>
                <p>Bạn có thể đăng nhập và bắt đầu khám phá ngay hôm nay!</p>
                <a href='https://yourapp.com/login' style='display: inline-block; padding: 10px 20px; color: white; background-color: #007bff; text-decoration: none; border-radius: 5px;'>Đăng nhập ngay</a>
                <p style='margin-top: 20px; color: #888;'>Rimdasilva Team</p>
            </div>";
    }
}