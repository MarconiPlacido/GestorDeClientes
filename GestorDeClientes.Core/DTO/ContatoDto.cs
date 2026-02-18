using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeClientes.Core.DTO
{
    public class ContatoDto
    {  
        [Required(ErrorMessage ="campo obrigatório")]
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        [EmailAddress]
        public String Email { get; set; } = string.Empty;
    }
}