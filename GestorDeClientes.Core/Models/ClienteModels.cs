using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestorDeClientes.Core.Models
{
    public class ClienteModels
    {   
        [Key]
        [Required(ErrorMessage ="campo obrigatório")]
        public Guid Id { get; set; }

        [Required(ErrorMessage ="campo obrigatório")]
        public Guid UsuarioId { get; set; } 
    
        [ForeignKey("UsuarioId")]
        public UsuarioModel? Usuario { get; set; }
    
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage ="campo obrigatório")]
        public string Documento { get; set; } = string.Empty;
        public string Endereço { get; set; } = string.Empty;
        public List<ContatoModel>? Contatos { get; set; }
    }
}