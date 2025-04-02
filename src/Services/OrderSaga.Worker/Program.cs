// src/Services/OrderSaga.Worker/Program.cs
using Microsoft.EntityFrameworkCore;
using OrderSaga.Worker.Consumers;
using OrderSaga.Worker.Data;
using OrderSaga.Worker.Orchestrator;
using OrderSaga.Worker.Repositories;
using OrderSaga.Worker.Services;
using OrderSaga.Worker.Services.Implementations;
using OrderSaga.Worker.Settings;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<SagaDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("SagaStateDb")));

        // Cấu hình
        services.Configure<RabbitMQSettings>(
            hostContext.Configuration.GetSection("RabbitMQ"));

        // Repositories
        services.AddScoped<ISagaStateRepository, SagaStateRepository>();

        // Service Clients
        services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client => {
            client.BaseAddress = new Uri(hostContext.Configuration["ServiceUrls:OrderService"]);
        });

        services.AddHttpClient<IInventoryServiceClient, InventoryServiceClient>(client => {
            client.BaseAddress = new Uri(hostContext.Configuration["ServiceUrls:InventoryService"]);
        });

        services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client => {
            client.BaseAddress = new Uri(hostContext.Configuration["ServiceUrls:PaymentService"]);
        });

        services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(client => {
            client.BaseAddress = new Uri(hostContext.Configuration["ServiceUrls:NotificationService"]);
        });

        // Orchestrator
        services.AddSingleton<ISagaOrchestrator, SagaOrchestrator>();

        // Background Services
        services.AddHostedService<OrderCreatedConsumer>();
    })
    .Build();

// Tạo database nếu chưa tồn tại
using (var scope = host.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<SagaDbContext>();
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

await host.RunAsync();
