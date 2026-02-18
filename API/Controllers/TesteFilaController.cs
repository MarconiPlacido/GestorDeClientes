using GestorDeClientes.API.Services.RabbitPublicar;
using GestorDeClientes.Core.Contracts;
using GestorDeClientes.Core.Repositorios.Mongo;
using Microsoft.AspNetCore.Mvc;

namespace GestorDeClientes.API.Controllers{

[ApiController]
[Route("api/fila")]
public class TesteFilaController : ControllerBase
{
    private readonly IRabbitMqPublisher _publisher;
    private readonly IImportacaoRepository _importacaoRepository;

    public TesteFilaController(IRabbitMqPublisher publisher, IImportacaoRepository importacaoRepository)
    {
        _publisher = publisher;
        _importacaoRepository = importacaoRepository;
    }

    [HttpPost("importar")]
        public async Task<IActionResult> Importar()
        {
            var idImportacao = Guid.NewGuid();
            var idUsuario = Guid.NewGuid(); // depois vem do JWT

            var importacao = new ImportacaoDocument
            {
                Id = idImportacao,
                IdUsuario = idUsuario,
                DataProcessamento = DateTime.UtcNow,
                Status = "Aguardando",
                TotalParaProcessar = 0,
                TotalProcessadoSucesso = 0,
                TotalProcessadoErro = 0,
                Mensagem = "Importação criada"
            };

            await _importacaoRepository.CriarAsync(importacao);

            var mensagem = new ImportacaoMensagemDto
            {
                IdImportacao = idImportacao,
                IdUsuario = idUsuario,
                Bucket = "importacoes",
                Arquivo = "clientes.xlsx"
            };

            _publisher.Publicar("importacao-excel", mensagem);

            return Ok(new
            {
                idImportacao,
                status = "Aguardando"
            });
        }


}
}