using GestorDeClientes.Core.Dto;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Services;
using GestorDeClientes.Core.Services.importacaoCorrigida;
using GestorDeClientes.Core.Services.MinIo;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GestorDeClientes.Worker;

public class Worker : BackgroundService
{
    
    private readonly ILogger<Worker> _logger;
    private readonly IRabbitService _rabbit;
    private readonly IMinio _minio;
    private readonly IProcessadorImportacao _processador;

    public Worker(
        ILogger<Worker> logger,
        IRabbitService rabbit,
        IMinio minio,
        IProcessadorImportacao processador)
    {
        _logger = logger;
        _rabbit = rabbit;
        _minio = minio;
        _processador = processador;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker iniciado");

        _rabbit.Consumir<ImportacaoMensagemDto>(async msg =>
        {
            _logger.LogInformation("Processando importação {id}", msg.ImportacaoId);

            var arquivo = await _minio.DownloadAsync(
                "importacoes",
                msg.NomeArquivo
            );

            await _processador.ProcessarAsync(
                arquivo,
                msg.UsuarioId,
                msg.ImportacaoId
            );
        });

        return Task.CompletedTask;
    }
}
