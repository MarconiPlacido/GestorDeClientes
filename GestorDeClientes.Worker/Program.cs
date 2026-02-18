using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.Repositorios.Mongo;
using GestorDeClientes.Core.Services;
using GestorDeClientes.Core.Services.importacaoCorrigida;
using GestorDeClientes.Core.Services.MinIo;
using Microsoft.EntityFrameworkCore;
using Minio;
using RabbitMQ.Client;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IConnection>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<Program>>();

    var factory = new ConnectionFactory
        {
            HostName = "rabbitmq",
            UserName = "gestor",
            Password = "gestor123",
            DispatchConsumersAsync = true
        };


    const int maxRetries = 10;
    var delay = TimeSpan.FromSeconds(3);

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            logger.LogInformation(
                "Tentando conectar ao RabbitMQ (tentativa {Attempt}/{Max})",
                attempt, maxRetries);

            return factory.CreateConnection();
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "Falha ao conectar no RabbitMQ. Tentando novamente em {Delay}s",
                delay.TotalSeconds);

            Thread.Sleep(delay);
        }
    }

    throw new Exception("Não foi possível conectar ao RabbitMQ após várias tentativas.");
});

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IImportacaoRepository, ImportacaoRepository>();


builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// MinIO
builder.Services.AddSingleton<IMinioClient>(_ =>
    new MinioClient()
        .WithEndpoint("minio:9000")
        .WithCredentials("admin", "admin123")
        .WithSSL(false)
        .Build()
);

// Core services
builder.Services.AddScoped<IMinio, MinioService>();
builder.Services.AddScoped<IProcessadorImportacao, ProcessadorImportacao>();

// Worker
builder.Services.AddHostedService<ImportacaoWorker>();

var host = builder.Build();
host.Run();
