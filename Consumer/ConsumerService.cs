using Confluent.Kafka;

namespace Consumer
{
    public class ConsumerService(ILogger<ConsumerService> logger) : BackgroundService
    {
        ConsumerConfig consumerConfig = new ConsumerConfig
        {
            //группа подписчка. Одинаковые сообщения распараллеливаются по подписчикам внутри группы
            GroupId = "test-consumer-group",

            BootstrapServers = "broker:29092",
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
                            var consumeResult = consumer.Consume(stoppingToken);
                            var m = consumeResult.Message;

                            logger.LogInformation($@"[Consumed1] Topic: {consumeResult.Topic} [{m.Key} : {m.Value}] 
(Partition: {consumeResult.Partition}, Offset: {consumeResult.Offset})");
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