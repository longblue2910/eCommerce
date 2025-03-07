using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace SharedKernel.Email;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body);
    Task SendPasswordResetEmailAsync(string email, string resetToken);
}

public class EmailService(IOptions<EmailSettings> settings) : IEmailService
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;
        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        email.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var resetLink = $"{_settings.ResetPasswordUrl}?token={resetToken}";
        var subject = "Password Reset Request";
        var body = $@"
            <p>Click the link below to reset your password:</p>
            <a href='{resetLink}'>{resetLink}</a>";

        await SendEmailAsync(email, subject, body);

        /*
         * 
         *  
         *      📌 TODO: Xử lý Reset Password trên Web
                -------------------------------------------------------------------
                1️. Frontend - Khi User Nhấn Vào Link Reset Password
                ✅ URL: https://your-app.com/reset-password?token=xxxxx
                ✅ Hành động:
                
                    ✅ Lấy token từ URL (token=xxxxx).
                    ✅ Hiển thị form nhập mật khẩu mới.
                    ✅ Kiểm tra token có tồn tại không, nếu không có thì hiển thị lỗi.
                💡 Giao diện Reset Password Form:              
                    📝 Nhập mật khẩu mới (New Password).
                    🔒 Nhập lại mật khẩu (Confirm Password).
                    🔘 Nút "Reset Password".
                -------------------------------------------------------------------
                2. Frontend - Gửi Yêu Cầu Reset Password
                ✅ API Call: 
                    Gửi request POST /api/auth/reset-password
                    {
                      "token": "xxxxx",
                      "newPassword": "your-new-password"
                    }
                ✅ Xử lý phản hồi:
                 🔹 Nếu thành công → Hiển thị thông báo "Mật khẩu đã được đặt lại thành công. Vui lòng đăng nhập lại." và chuyển hướng đến trang login.
                 🔹 Nếu lỗi → Hiển thị thông báo lỗi (Token không hợp lệ hoặc đã hết hạn.).
                -------------------------------------------------------------------
                3️. Backend - Xác Thực & Đặt Lại Mật Khẩu
                ✅ Bước 1: Kiểm tra token có hợp lệ không?
                ✅ Bước 2: Giải mã token để lấy userId.
                ✅ Bước 3: Kiểm tra user có tồn tại không?
                ✅ Bước 4: Hash mật khẩu mới và cập nhật vào database.
                ✅ Bước 5: Trả về Result.Success().
                -------------------------------------------------------------------
                4. Frontend - Hiển Thị Kết Quả
                ✅ Nếu thành công:
                    🌟 Hiển thị thông báo "Mật khẩu đã thay đổi thành công. Đang chuyển hướng..."
                    🔄 Chuyển hướng đến trang login sau 3 giây.
                ✅ Nếu lỗi:             
                    ❌ "Token đã hết hạn hoặc không hợp lệ."
                    ❌ "Mật khẩu không được để trống."
                    ❌ "Mật khẩu quá yếu, hãy chọn mật khẩu mạnh hơn."         
         */
    }
}
