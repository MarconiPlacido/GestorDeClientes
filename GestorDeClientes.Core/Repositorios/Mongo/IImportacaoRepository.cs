namespace GestorDeClientes.Core.Repositorios.Mongo{
public interface IImportacaoRepository
{
    Task CriarAsync(ImportacaoDocument importacao);
    Task AtualizarAsync(ImportacaoDocument importacao);
    Task<ImportacaoDocument> ObterPorIdAsync(Guid id);
}
}
