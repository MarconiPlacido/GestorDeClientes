using GestorDeClientes.Core.Dto;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;

namespace GestorDeClientes.Core.Services.Cadastrar
{
    public interface ICadastrar
    {
        Task<ResponseModels<UsuarioModel>> Cadastrar ( CriarUsuarioDto Usuario);
        public bool VerificarExistencia(CriarUsuarioDto UsuarioVerificar);
        Task<ResponseModels<string>> Login ( LoginDto UsuarioDto);
        
    }
}