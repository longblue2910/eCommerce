using OrderSaga;
using OrderSaga.Messaging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
        services.AddSingleton<OrderSagaOrchestrator>();
        services.AddHostedService<OrderSagaWorker>();
    })
    .Build();

await host.RunAsync();
