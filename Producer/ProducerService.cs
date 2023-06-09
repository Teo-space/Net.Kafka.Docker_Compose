using Confluent.Kafka;

namespace Producer
{
    public class ProducerService(ILogger<ProducerService> logger) : BackgroundService
    {
        ProducerConfig producerConfig = new ProducerConfig
        {
            BootstrapServers = "broker:29092",
        };

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Publisher Start");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(400, stoppingToken);
                //await ProduceAsync(producerConfig);
                Produce(producerConfig);
            }
        }

        //You should use the ProduceAsync method if you would like to wait for the result of your produce requests before proceeding. 
        //You might typically want to do this in highly concurrent scenarios, for example in the context of handling web requests. 
        //Behind the scenes, the client will manage optimizing communication with the Kafka brokers for you, batching requests as appropriate.
        async Task ProduceAsync(ProducerConfig config)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                var message = new Message<string, string>() { Key = "Message Key", Value = "Message Value" };
                var result = await producer.ProduceAsync("Kafka-Topic-Sample1", message);
                logger.LogInformation($"Produced: {result.Topic} [{result.Key} : {result.Value}] (Partition:{result.Partition.Value}, Offset:{result.Offset.Value})");
            }
        }


        Action<DeliveryReport<string, string>> handler = (report) =>
        {
            if (report.Error.IsError)
            {
                logger.LogInformation($"Delivery Error: {report.Error.Reason}");
            }
            else
            {
                logger.LogInformation($"Produced:  {report.Topic} [{report.Key} : {report.Value}] {report.Partition.Value}, {report.Offset.Value}");
            }
        };

        //In stream processing applications, where you would like to process many messages in rapid succession,
        //you would typically use the Produce method instead
        void Produce(ProducerConfig config)
        {
            using (var producer = new ProducerBuilder<string, string>(config).Build())
            {
                for (int i = 0; i < 30; ++i)
                {
                    var message = new Message<string, string>() { Key = "Message Key", Value = $"Message Value {i}" };

                    producer.Produce("Kafka-Topic-Sample1", message, handler);

                    Task.Delay(200).Wait();
                }

                producer.Flush(TimeSpan.FromSeconds(10));
            }
        }

    }
}


