using System.ComponentModel.DataAnnotations;

namespace GestorDeClientes.Core.DTO
{
    public class CriarUsuarioDto
    {
        [Required(ErrorMessage ="campo obrigatório")]
         public string? Nome { get; set; }
        [Required(ErrorMessage ="campo obrigatório")]
        public string? Cpf { get; set; }
        [Required(ErrorMessage ="campo obrigatório")]
        public string? Login { get; set; }
        [Required(ErrorMessage ="campo obrigatório")]
        public string? Senha { get; set; }
        [Compare ("Senha", ErrorMessage ="as senhas não são iguais")]
        public string? ConfirmarSenha { get; set; }
    }
}