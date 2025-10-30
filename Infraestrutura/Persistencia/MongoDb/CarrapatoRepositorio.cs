using Dominio.Persistencia;
using MongoDB.Driver;


namespace Infraestrutura.Persistencia.MongoDb
{
    public class CarrapatoRepositorio
    {
        private readonly IMongoCollection<Carrapato> _collection;

        public CarrapatoRepositorio(MongoDbContext context)
        {
            _collection = context.Database.GetCollection<Carrapato>("Carrapatos");
        }

        public Task<List<Carrapato>> ObterTodosAsync()
        {
            return _collection.Find(_ => true).ToListAsync();
        }

        public Task<Carrapato?> ObterPorCodigoSerialAsync(string codigoSerial)
        {
            return _collection.Find(c => c.CodigoSerial == codigoSerial).FirstOrDefaultAsync();
        }

        public async Task AdicionarAsync(Carrapato carrapato)
        {
            await _collection.InsertOneAsync(carrapato);
        }

        public async Task AtualizarAsync(Carrapato carrapato)
        {
            await _collection.ReplaceOneAsync(c => c.CodigoSerial == carrapato.CodigoSerial, carrapato);
        }

        public async Task DeletarAsync(string codigoSerial)
        {
            await _collection.DeleteOneAsync(c => c.CodigoSerial == codigoSerial);
        }


    }
}
