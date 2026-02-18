using System.ComponentModel.DataAnnotations;

namespace GestorDeClientes.Core.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage ="Campo Obrigatorio")]
        public String? Login { get; set; } 
        [Required(ErrorMessage ="campo obrigatório")]
        public string? Senha { get; set; }
    }
}