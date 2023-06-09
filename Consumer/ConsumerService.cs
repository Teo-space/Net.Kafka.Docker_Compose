namespace Consumer
{
    public class ConsumerService(ILogger<ConsumerService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Work: {time}", DateTimeOffset.Now);


                await Task.Delay(1000, stoppingToken);



            }
        }
    }
}