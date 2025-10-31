using Dominio.Persistencia;
using Infraestrutura.Repositorios.MongoDb;

namespace Aplicacao.Servicos
{
    public class CarrapatoMongoService
    {
        private readonly CarrapatoMongoRepositorio _repositorio;

        public CarrapatoMongoService(CarrapatoMongoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public async Task<List<CarrapatoMongo>> ObterTodosAsync() =>
            await _repositorio.ObterTodosAsync();

        public async Task<CarrapatoMongo?> ObterPorCodigoSerialAsync(string codigoSerial) =>
            await _repositorio.ObterPorCodigoSerialAsync(codigoSerial);

        public async Task<CarrapatoMongo> AdicionarAsync(CarrapatoMongo carrapato) =>
            await _repositorio.AdicionarAsync(carrapato);

        public async Task<CarrapatoMongo?> AtualizarAsync(string codigoSerial, CarrapatoMongo carrapato) =>
            await _repositorio.AtualizarAsync(codigoSerial, carrapato);

        public async Task DeletarAsync(string codigoSerial) =>
            await _repositorio.DeletarAsync(codigoSerial);
    }
}