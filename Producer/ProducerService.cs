namespace Producer
{
    public class ProducerService(ILogger<ProducerService> logger) : BackgroundService
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