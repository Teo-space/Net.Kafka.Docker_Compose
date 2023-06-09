using Producer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<ProducerService>();
    })
    .Build();

host.Run();
