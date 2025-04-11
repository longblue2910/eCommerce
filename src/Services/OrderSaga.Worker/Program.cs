// src/Services/OrderSaga.Worker/Program.cs
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderSaga.Worker.Consumers;
using OrderSaga.Worker.Data;
using OrderSaga.Worker.Orchestrator;
using OrderSaga.Worker.Repositories;
using OrderSaga.Worker.Services;
using OrderSaga.Worker.Services.Implementations;
using SharedKernel.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddDbContext<SagaDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString("SagaStateDb")));

        // Cấu hình
        services.Configure<RabbitMQSettings>(
            hostContext.Configuration.GetSection("RabbitMQ"));

        // src/Services/OrderSaga.Worker/Program.cs
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqSettings = hostContext.Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();

                cfg.Host(new Uri($"rabbitmq://{rabbitMqSettings.Host}:{rabbitMqSettings.Port}/"), h =>
                {
                    h.Username(rabbitMqSettings.Username);
                    h.Password(rabbitMqSettings.Password);
                });

                // Tùy chỉnh deserializer để xử lý format message từ RabbitMqPublisher
                cfg.UseRawJsonSerializer();
                cfg.UseRawJsonDeserializer();

                cfg.ReceiveEndpoint("order_saga_queue", e =>
                {
                    e.Bind("order_saga_exchange", x =>
                    {
                        x.RoutingKey = "order.created";
                        x.ExchangeType = "direct";
                    });

                    e.ConfigureConsumer<OrderCreatedConsumer>(context);
                });
            });
        });



        // Repositories
        services.AddScoped<ISagaStateRepository, SagaStateRepository>();

        // Service Clients
        services.AddHttpClient<IOrderServiceClient, OrderServiceClient>(client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ServiceUrls:OrderService"]);
        });

        services.AddSingleton<IInventoryServiceClient, InventoryServiceClient>();

        services.AddHttpClient<IPaymentServiceClient, PaymentServiceClient>(client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ServiceUrls:PaymentService"]);
        });

        services.AddHttpClient<INotificationServiceClient, NotificationServiceClient>(client =>
        {
            client.BaseAddress = new Uri(hostContext.Configuration["ServiceUrls:NotificationService"]);
        });

        // Orchestrator
        services.AddSingleton<ISagaOrchestrator, SagaOrchestrator>();
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
