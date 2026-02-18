using System.ComponentModel.DataAnnotations;

namespace GestorDeClientes.Core.Models
{
    public class UsuarioModel
    {   [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string? Nome { get; set; }
        [Required]
        public string? Cpf { get; set; }
        [Required]
        public string? Login { get; set; }
        [Required]
        public byte[]? SenhaHash { get; set; }
        public byte[]? SenhaSalt{get; set;}
        public DateTime DataDaCriacao { get; set; } = DateTime.Now;

        public List<ClienteModels> Clientes = new();

    }
}