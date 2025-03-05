using API.Middlewares;
using Application.Services;
using Infrastructure;
using SharedKernel.Email;
using Infrastructure.Persistence;
using Application.Commands.User;
using Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Current Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");


builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 🔹 1️ Đăng ký Controller & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


// 🔹 2️ Đăng ký các dịch vụ hạ tầng (Repository, DbContext, ...)
builder.Services.AddInfrastructure(builder.Configuration);

// 🔹 3️ Đăng ký MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

// 🔹 4️ Cấu hình EmailSettings trước khi DI
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

var app = builder.Build();

// 🛠 Thực hiện Migration + Seed Data trước khi chạy Middleware
app.MigrateDatabase();

/*
 * Đăng ký Middleware theo thứ tự chuẩn: Exception -> Swagger -> HTTPS -> Auth -> Controllers.
 */
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();