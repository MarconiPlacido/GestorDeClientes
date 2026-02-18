namespace GestorDeClientes.Core.DTO
{
    public class ImportacaoMensagemDto
    {
        public Guid ImportacaoId { get; set; }
        public Guid UsuarioId { get; set; }
        public string Bucket { get; set; } = string.Empty;
        public string NomeArquivo { get; set; } = string.Empty;
        public DateTime DataImportacao { get; set; }

    }
}