using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace GestorDeClientes.Core.Services
{
    public class RabbitService : IRabbitService
    {
        private readonly IConnection _connection;

        public RabbitService(IConnection connection)
        {
            _connection = connection;
        }

        public void Publicar<T>(T mensagem)
        {
            using var channel = _connection.CreateModel();

            channel.QueueDeclare(
                queue: typeof(T).Name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(
                JsonSerializer.Serialize(mensagem)
            );

            channel.BasicPublish(
            exchange: "",
            routingKey: "importacao-excel",
            basicProperties: null,
            body: body
        );
        }

        public void Consumir<T>(Action<T> onMessage)
        {
            var channel = _connection.CreateModel();

            channel.QueueDeclare(
                queue: typeof(T).Name,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, ea) =>
            {
                var body = ea.Body.ToArray();
                var mensagemJson = Encoding.UTF8.GetString(body);

                var mensagem = JsonSerializer.Deserialize<T>(mensagemJson);

                onMessage(mensagem!);

                channel.BasicAck(ea.DeliveryTag, false);
            };

            channel.BasicConsume(
                queue: typeof(T).Name,
                autoAck: false,
                consumer: consumer
            );
        }
    }
}
