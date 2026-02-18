using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace GestorDeClientes.Core.Data;

public class MongoDbContext
{
    public IMongoDatabase Database { get; }

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration["Mongo:ConnectionString"]);
        Database = client.GetDatabase(configuration["Mongo:Database"]);
    }
}
