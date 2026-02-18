using System.ComponentModel.DataAnnotations;

namespace GestorDeClientes.Core.DTO
{
    public class ClienteDto
    {   
        [Required(ErrorMessage ="campo obrigatório")]
        public string Nome { get; set; } = string.Empty;
        [Required(ErrorMessage ="campo obrigatório")]
        public string Documento { get; set; } = string.Empty;
        public string Endereço { get; set; } = string.Empty;
    }
}