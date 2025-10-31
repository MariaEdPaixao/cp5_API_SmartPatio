using Infraestrutura.Configuracoes;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infraestrutura.Repositorios.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbConfiguracoes> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoDatabase Database => _database;

    }
}
