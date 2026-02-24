using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace AssetApi.Messaging;

public class RabbitMqPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string Exchange = "efams";

    public RabbitMqPublisher(IConfiguration config)
    {
        var factory = new ConnectionFactory
        {
            HostName = config["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(config["RabbitMQ:Port"] ?? "5672")
        };

        var retries = 5;
        while (retries > 0)
        {
            try
            {
                _connection = factory.CreateConnection();
                break;
            }
            catch (Exception)
            {
                retries--;
                if (retries == 0) throw;
                Thread.Sleep(5000);
            }
        }

        _channel = _connection!.CreateModel();
        _channel.ExchangeDeclare(Exchange, ExchangeType.Topic, durable: true);
    }

    public void Publish<T>(string routingKey, T message)
    {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(Exchange, routingKey, null, body);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}