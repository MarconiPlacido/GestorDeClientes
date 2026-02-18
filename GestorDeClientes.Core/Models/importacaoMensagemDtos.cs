namespace GestorDeClientes.Core.Contracts{
    public class ImportacaoMensagemDto
    {
        public Guid IdImportacao { get; set; }
        public Guid IdUsuario { get; set; }
        public string Bucket { get; set; } = string.Empty;
        public string Arquivo { get; set; } = string.Empty;
    }
}