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
            var client = new MongoClient("mongodb://admin:admin123@localhost:27017/?authSource=admin");
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoDatabase Database => _database;

    }
}