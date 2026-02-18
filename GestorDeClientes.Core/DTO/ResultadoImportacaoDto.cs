namespace GestorDeClientes.Core.DTO
{
    public class ResultadoImportacaoDto
    {
        public int TotalLinhas { get; set; }
        public int ClientesCriados { get; set; }
        public int ContatosCriados { get; set; }
        public int ContatosIgnorados { get; set; }
        public List<string> Erros { get; set; } = new();
    }

}