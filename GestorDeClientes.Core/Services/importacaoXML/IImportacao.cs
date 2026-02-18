using GestorDeClientes.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace GestorDeClientes.Core.Services.importacaoXML
{
    public interface IImportacao
{
    Task<ResultadoImportacaoDto> ImportarClientesAsync(IFormFile arquivo, Guid UsuarioId);
}

}