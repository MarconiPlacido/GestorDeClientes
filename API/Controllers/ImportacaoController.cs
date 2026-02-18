using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using GestorDeClientes.Core.Services.CadastrarCliente;
using GestorDeClientes.Core.Services;
using GestorDeClientes.Core.Services.importacaoXML;
using GestorDeClientes.Core.Services.MinIo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GestorDeClientes.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImportacaoController : ControllerBase
    {
        private readonly IMinio _minio;
        private readonly IRabbitService _rabbit;

        private readonly IImportacao _importacaoService;
        private object _rabbitService;

        public ImportacaoController(IMinio minio, IRabbitService rabbit, IImportacao importacao)
        {
            _minio = minio;
            _rabbit = rabbit;
            _importacaoService = importacao;
        }




    [Authorize]
    [HttpPost("importa")]
    public async Task<IActionResult> ImportarClientes([FromForm] ImportacaoExcelDto dto)
    {
            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
            return BadRequest("Arquivo inválido.");

        var usuarioIdClaim = User.FindFirst("Id")?.Value;

           if (usuarioIdClaim == null)
            return Unauthorized("Usuário não autenticado.");

        var usuarioId = Guid.Parse(usuarioIdClaim);

        var resultado = await _importacaoService
                .ImportarClientesAsync(dto.Arquivo, usuarioId);

        return Ok(resultado);
        }




        [HttpPost("importar")]
        public async Task<IActionResult> Importar([FromForm] ImportacaoExcelDto dto)
        {
            if (dto.Arquivo == null || dto.Arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            var usuarioIdClaim = User.FindFirst("Id")?.Value;

            if (usuarioIdClaim == null)
                return Unauthorized();

            var usuarioId = Guid.Parse(usuarioIdClaim);

            // Nome único do arquivo
            var nomeArquivo = $"{Guid.NewGuid()}_{dto.Arquivo.FileName}";

            // Upload no MinIO
            using var stream = dto.Arquivo.OpenReadStream();

            await _minio.UploadAsync(
                bucket: "importacoes",
                objectName: nomeArquivo,
                fileStream: stream,
                contentType: dto.Arquivo.ContentType
            );

            var mensagem = new ImportacaoMensagemDto
            {
                ImportacaoId = Guid.NewGuid(),
                UsuarioId = usuarioId,
                Bucket = "importacoes",
                NomeArquivo = nomeArquivo,
                DataImportacao = DateTime.UtcNow
            };

            _rabbit.Publicar(mensagem);


            return Accepted(new
            {
                mensagem = "Importação enviada para processamento",
                mensagemId = mensagem.ImportacaoId
            });
        }
    }
}
