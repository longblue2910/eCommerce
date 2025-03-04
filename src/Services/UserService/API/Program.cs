using API.Middlewares;
using Application.Commands;
using Application.Services;
using Infrastructure;
using SharedKernel.Email;

var builder = WebApplication.CreateBuilder(args);

// 🔹 1️ Đăng ký Controller & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 2️ Đăng ký các dịch vụ hạ tầng (Repository, DbContext, ...).
builder.Services.AddInfrastructure(builder.Configuration);

// 🔹 3️ Đăng ký MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

// 🔹 4️ Cấu hình EmailSettings trước khi DI
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

var app = builder.Build();

/*
 * Đăng ký Middleware theo thứ tự chuẩn: Exception -> Swagger -> HTTPS -> Auth -> Controllers.
 */

// 🔹 5️ Middleware xử lý Exception phải đặt sớm nhất!
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 🔹 6️ Swagger (chỉ khi Dev mode)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🔹 7️ Cấu hình Middleware quan trọng khác
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
