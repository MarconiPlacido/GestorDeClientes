using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;

namespace GestorDeClientes.Core.Services
{
    public interface Icontato
    {
        
        Task<bool> VerificarExistencia(ContatoDto ContatoVerificar);

        Task<ResponseModels<List<ContatoModel>>> BuscarContato (string documentoDoCliente);

        Task<ResponseModels<ContatoModel>> CriarContato(ContatoDto Contato, string Documento);

        Task<ResponseModels<ContatoModel>> BuscarContatoPorId(Guid Id);

        Task<ResponseModels<ContatoModel>> EditarContato (Guid Id, ContatoDto contatoAtualizado);

        Task<ResponseModels<ContatoModel>> ExcluirContato (Guid Id);


    }

}