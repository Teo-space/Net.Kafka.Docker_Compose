using Confluent.Kafka;

namespace Consumer
{
    public class ConsumerService(ILogger<ConsumerService> logger) : BackgroundService
    {
        ConsumerConfig consumerConfig = new ConsumerConfig
        {
            //группа подписчка. Одинаковые сообщения распараллеливаются по подписчикам внутри группы
            GroupId = "test-consumer-group",

            BootstrapServers = "kafka:29092",
            //BootstrapServers = "localhost:9092",

            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Started");

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe("Kafka-Topic-Sample1");//Подписка на топик

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(600, stoppingToken);
                        try
                        {
                            var cr = consumer.Consume(stoppingToken);

                            logger.LogInformation($"[Consumed1] Topic: {cr.Topic} [{cr.Key} : {cr.Value}] (Partition: {cr.Partition.Value}, Offset: {cr.Offset.Value})");
                        }
                        catch (ConsumeException e)
                        {
                            logger.LogError($"Error occured: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException) // Ensure the consumer leaves the group cleanly and final offsets are committed.
                {
                    consumer.Close();
                }
            }
        }




    }
}