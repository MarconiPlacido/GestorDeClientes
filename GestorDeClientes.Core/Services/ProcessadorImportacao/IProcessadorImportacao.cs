using GestorDeClientes.Core.DTO;

namespace GestorDeClientes.Core.Services.importacaoCorrigida
{
    public interface IProcessadorImportacao
    {
        Task<ResultadoImportacaoDto> ProcessarAsync(
            Stream stream,
            Guid usuarioId,
            Guid importacaoId
        );
    }

}