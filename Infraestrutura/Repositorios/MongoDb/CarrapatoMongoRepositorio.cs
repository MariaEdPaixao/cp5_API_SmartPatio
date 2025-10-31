using Dominio.Persistencia;
using MongoDB.Driver;


namespace Infraestrutura.Repositorios.MongoDb
{
    public class CarrapatoMongoRepositorio
    {
        private readonly IMongoCollection<Carrapato> _collection;

        public CarrapatoMongoRepositorio(MongoDbContext context)
        {
            _collection = context.Database.GetCollection<Carrapato>("Carrapatos");
        }

        public async Task<List<Carrapato>> ObterTodosAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Carrapato?> ObterPorCodigoSerialAsync(string codigoSerial)
        {
            return await _collection.Find(c => c.CodigoSerial == codigoSerial).FirstOrDefaultAsync();
        }

        public async Task<Carrapato> AdicionarAsync(Carrapato carrapato)
        {
            await _collection.InsertOneAsync(carrapato);
            return await ObterPorCodigoSerialAsync(carrapato.CodigoSerial) ?? carrapato;
        }

        public async Task<Carrapato?> AtualizarAsync(string codigoSerial, Carrapato carrapato)
        {
            await _collection.ReplaceOneAsync(c => c.CodigoSerial == codigoSerial, carrapato);
            return await ObterPorCodigoSerialAsync(codigoSerial);
        }

        public async Task DeletarAsync(string codigoSerial)
        {
            await _collection.DeleteOneAsync(c => c.CodigoSerial == codigoSerial);
        }


    }
}
