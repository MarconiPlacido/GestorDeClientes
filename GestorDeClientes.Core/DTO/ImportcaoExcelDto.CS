using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GestorDeClientes.Core.DTO
{
    public class ImportacaoExcelDto
{
    [Required]
    public IFormFile? Arquivo { get; set; } = null!;
    
}

}