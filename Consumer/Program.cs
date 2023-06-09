using Consumer;

Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services => services.AddHostedService<ConsumerService>())
    .Build()
    .Run();
