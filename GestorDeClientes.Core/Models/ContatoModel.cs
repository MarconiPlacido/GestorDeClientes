using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeClientes.Core.Models
{
    public class ContatoModel
    {   [Key]
        [Required(ErrorMessage ="campo obrigatório")]
        public Guid  Id  { get; set; }

        [Required(ErrorMessage ="campo obrigatório")]
        public Guid ClienteId { get; set; } 
    
        [ForeignKey("ClienteId")]
        public ClienteModels? Cliente { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        [EmailAddress]
        public String Email { get; set; } = string.Empty;
    }
}