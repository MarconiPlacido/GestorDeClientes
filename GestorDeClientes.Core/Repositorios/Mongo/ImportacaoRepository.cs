
using GestorDeClientes.Core.Data;
using MongoDB.Driver;


namespace GestorDeClientes.Core.Repositorios.Mongo{
public class ImportacaoRepository : IImportacaoRepository
{
    private readonly IMongoCollection<ImportacaoDocument> _collection;

    public ImportacaoRepository(MongoDbContext context)
    {
        _collection = context.Database.GetCollection<ImportacaoDocument>("Importacoes");
    }

    public Task CriarAsync(ImportacaoDocument importacao) =>
        _collection.InsertOneAsync(importacao);

    public Task AtualizarAsync(ImportacaoDocument importacao) =>
        _collection.ReplaceOneAsync(x => x.Id == importacao.Id, importacao);

    public Task<ImportacaoDocument> ObterPorIdAsync(Guid id) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
}
}