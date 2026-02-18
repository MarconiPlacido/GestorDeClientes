using GestorDeClientes.API.Services.RabbitPublicar;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
namespace GestorDeClientes.API.Services.RabbitPublicar
{

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IConnection _connection;

    public RabbitMqPublisher(IConnection connection)
    {
        _connection = connection;
    }

    public void Publicar<T>(string fila, T mensagem)
    {
        using var channel = _connection.CreateModel();

        channel.QueueDeclare(
            queue: fila,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var json = JsonSerializer.Serialize(mensagem);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(
            exchange: "",
            routingKey: fila,
            basicProperties: null,
            body: body
        );
    }
}
}
