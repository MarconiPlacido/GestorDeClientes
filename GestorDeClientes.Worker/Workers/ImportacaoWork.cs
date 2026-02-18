using System.Text;
using System.Text.Json;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Services.importacaoCorrigida;
using GestorDeClientes.Core.Services.ImportacaoHistorico;
using GestorDeClientes.Core.Services.MinIo;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class ImportacaoWorker : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IServiceScopeFactory _scopeFactory;

    public ImportacaoWorker(
        IConnection connection,
        IServiceScopeFactory scopeFactory)
    {
        _connection = connection;
        _scopeFactory = scopeFactory;

        _channel = _connection.CreateModel();


        _channel.QueueDeclare(
            queue: "importacao-excel",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        Console.WriteLine("ImportacaoWorker iniciado");


    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (sender, ea) =>
        {
            Console.WriteLine("🔥 Mensagem recebida");

            try
            {
                var body = ea.Body.ToArray();
                var mensagemJson = Encoding.UTF8.GetString(body);

                var mensagem = JsonSerializer.Deserialize<ImportacaoMensagemDto>(mensagemJson);

                Console.WriteLine($"🔥 Processando {mensagem.ImportacaoId}");

                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erro: {ex}");
                _channel.BasicNack(ea.DeliveryTag, false, false);
            }
        };

        _channel.BasicConsume(
            queue: "importacao-excel",
            autoAck: false,
            consumer: consumer
        );

        Console.WriteLine("Worker escutando a fila importacao-excel");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

}
