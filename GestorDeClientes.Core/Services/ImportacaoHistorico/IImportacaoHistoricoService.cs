namespace GestorDeClientes.Core.Services.ImportacaoHistorico
{
    public interface IImportacaoHistoricoService
{
    Task CriarImportacaoAsync(Guid id, Guid usuarioId);
    Task AtualizarStatusAsync(Guid id, string status, string? mensagem = null);
    Task AtualizarTotaisAsync(Guid id,
        int clientesCriados,
        int contatosCriados,
        int contatosIgnorados);

    Task AdicionarItemAsync(Guid importacaoId, int linha, string status, string? mensagem);
}

}