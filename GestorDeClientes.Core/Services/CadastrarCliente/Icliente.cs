using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;

namespace GestorDeClientes.Core.Services.CadastrarCliente
{
    public interface Icliente
    {
        Task<bool> VerificarExistencia(ClienteDto ClienteVerificar);
        Task<ResponseModels<ClienteModels>> CriarCliente(ClienteDto Cliente, Guid usuarioId);

        Task<ClienteModels> BuscarCliente(string Documento);

        Task<ResponseModels<ClienteModels>> BuscarClientePorId(string Documento);

        Task<ResponseModels<ClienteModels>> EditarCliente (string Documento, ClienteDto clienteAtualizado);

        Task<ResponseModels<ClienteModels>> ExcluirCliente (string Documento);
    }
}