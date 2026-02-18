namespace GestorDeClientes.Core.Services.ImportacaoHistorico
{
    using GestorDeClientes.Core.Data;
    using GestorDeClientes.Core.Models;
    using MongoDB.Driver;

    public class ImportacaoHistoricoService : IImportacaoHistoricoService
    {
        private readonly IMongoCollection<ImportacaoDocument> _importacoes;
        private readonly IMongoCollection<ItemImportacaoDocument> _itens;

        public ImportacaoHistoricoService(MongoDbContext context)
        {
            _importacoes = context.Database
                .GetCollection<ImportacaoDocument>("Importacoes");

            _itens = context.Database
                .GetCollection<ItemImportacaoDocument>("ItensImportacao");
        }

        public async Task CriarImportacaoAsync(Guid id, Guid usuarioId)
        {
            var doc = new ImportacaoDocument
            {
                Id = id,
                IdUsuario = usuarioId,
                DataProcessamento = DateTime.UtcNow,
                Status = "Recebido"
            };

            await _importacoes.InsertOneAsync(doc);
        }

        public async Task AtualizarStatusAsync(Guid id, string status, string? mensagem = null)
        {
            var update = Builders<ImportacaoDocument>.Update
                .Set(x => x.Status, status)
                .Set(x => x.Mensagem, mensagem);

            await _importacoes.UpdateOneAsync(
                x => x.Id == id,
                update
            );
        }

        public async Task AtualizarTotaisAsync(Guid id,
            int clientesCriados,
            int contatosCriados,
            int contatosIgnorados)
        {
            var update = Builders<ImportacaoDocument>.Update
                .Set(x => x.TotalProcessadoSucesso, clientesCriados)
                .Set(x => x.TotalParaProcessar, contatosCriados)
                .Set(x => x.TotalProcessadoErro, contatosIgnorados);

            await _importacoes.UpdateOneAsync(
                x => x.Id == id,
                update
            );
        }

        public async Task AdicionarItemAsync(Guid importacaoId, int linha, string status, string? mensagem)
        {
            var item = new ItemImportacaoDocument
            {
                Id = Guid.NewGuid(),
                IdImportacao = importacaoId,
                Status = status,
                Mensagem = mensagem
            };

            await _itens.InsertOneAsync(item);
        }
    }
}
