using Dominio.Persistencia;
using MongoDB.Driver;


namespace Infraestrutura.Repositorios.MongoDb
{
    public class CarrapatoMongoRepositorio
    {
        private readonly IMongoCollection<CarrapatoMongo> _collection;

        public CarrapatoMongoRepositorio(MongoDbContext context)
        {
            _collection = context.Database.GetCollection<CarrapatoMongo>("Carrapatos");
        }
        public async Task<List<CarrapatoMongo>> ObterTodosAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<CarrapatoMongo?> ObterPorCodigoSerialAsync(string codigoSerial)
        {
            return await _collection.Find(c => c.CodigoSerial == codigoSerial).FirstOrDefaultAsync();
        }

        public async Task<CarrapatoMongo> AdicionarAsync(CarrapatoMongo carrapato)
        {
            await _collection.InsertOneAsync(carrapato);
            return await ObterPorCodigoSerialAsync(carrapato.CodigoSerial) ?? carrapato;
        }

        public async Task<CarrapatoMongo?> AtualizarAsync(string codigoSerial, CarrapatoMongo carrapato)
        {
            var existente = await _collection.Find(c => c.CodigoSerial == codigoSerial).FirstOrDefaultAsync();
            if (existente is null) return null;

            carrapato.Id = existente.Id;

            await _collection.ReplaceOneAsync(c => c.Id == existente.Id, carrapato);
            return carrapato;
        }

        public async Task DeletarAsync(string codigoSerial)
        {
            await _collection.DeleteOneAsync(c => c.CodigoSerial == codigoSerial);
        }
    }
}
