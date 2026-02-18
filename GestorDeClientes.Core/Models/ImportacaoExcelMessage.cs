namespace GestorDeClientes.Core.Contracts
{
    public class ImportacaoExcelMessage
    {
        public Guid ImportacaoId { get; set; }
        public string Bucket { get; set; } = default!;
        public string ObjectName { get; set; } = default!;
        public DateTime DataEnvio { get; set; }
    }
}
