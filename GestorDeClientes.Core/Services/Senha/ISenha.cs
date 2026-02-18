using GestorDeClientes.Core.Models;

namespace GestorDeClientes.Core.Services.Senha
{
    public interface ISenha
    {
        public void CriarSenhaHash(string Senha, out byte [] SenhaHash, out byte [] SenhaSalt);
        string CriarToken(UsuarioModel usuario);
        bool VerificarSenha (string Senha, byte [] SenhaHash, byte [] SenhaSalt);
    }
}