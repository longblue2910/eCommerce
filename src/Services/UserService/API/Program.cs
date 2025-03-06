using API;
using API.Middlewares;
using Application.Commands.User;
using Application.Mappings;
using Application.Services;
using Infrastructure;
using Infrastructure.Options;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SharedKernel.Email;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine($"Current Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// 🔹 1️ Đăng ký Controller & Swagger
builder.Services.AddControllers();

var jwtSettings = builder.Configuration.GetSection(nameof(JwtSettings))
    .Get<JwtSettings>();

builder.Services.AddSingleton(jwtSettings);


builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigningKey,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings.IssuerSigningKey)),
        ValidateIssuer = jwtSettings.ValidateIssuer,
        ValidIssuer = jwtSettings.ValidIssuer,
        ValidateAudience = jwtSettings.ValidateAudience,
        ValidAudience = jwtSettings.ValidAudience,
        RequireExpirationTime = jwtSettings.RequireExpirationTime,
        ValidateLifetime = jwtSettings.RequireExpirationTime,
        ClockSkew = TimeSpan.FromSeconds(10),
    };
});

SwaggerConfig.Configure(builder.Services);

builder.Services.AddHttpClient();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// 🔹 4️ Đăng ký MediatR (CQRS)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

// 🔹 5️ Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(UserProfile));

builder.Services.AddOptions();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc();

// 🔹 6️ Cấu hình EmailSettings trước khi DI
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// 🔹 3️ Đăng ký các dịch vụ hạ tầng (Repository, DbContext, ...)
builder.Services.AddInfrastructure(builder.Configuration);

// 🔹 7️ Cấu hình CORS (Cross-Origin Requests)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// 🛠 Thực hiện Migration + Seed Data trước khi chạy Middleware
app.MigrateDatabase();

/*
 * Đăng ký Middleware theo thứ tự chuẩn:
 * Exception -> Swagger -> HTTPS -> Auth -> Logging -> RBAC Middleware -> Controllers.
 */
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1");
        c.DocumentTitle = "API Documentation";
    });
}

app.UseHttpsRedirection();

app.UseRouting();

// 🛠 Kích hoạt CORS
app.UseCors("AllowAllOrigins");


app.UseAuthorization();
app.UseAuthentication();


app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapDefaultControllerRoute();
});

app.Run();
