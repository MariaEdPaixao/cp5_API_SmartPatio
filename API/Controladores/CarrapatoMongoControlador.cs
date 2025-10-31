using Aplicacao.DTOs.Carrapato;
using Aplicacao.Servicos;
using Dominio.Persistencia;
using Microsoft.AspNetCore.Mvc;

namespace API.Controladores
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/mongo/[controller]")]
    [Tags("Carrapatos Mongo")]
    public class CarrapatoMongoControlador : ControllerBase
    {
        private readonly CarrapatoMongoService _service;

        public CarrapatoMongoControlador(CarrapatoMongoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retorna todos os carrapatos cadastrados no MongoDB.
        /// </summary>
        /// <returns>Lista de carrapatos.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<CarrapatoLeituraDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CarrapatoLeituraDto>>> GetTodos()
        {
            var carrapatos = await _service.ObterTodosAsync();
            var dtos = carrapatos.Select(c => MapearParaDto(c)).ToList();
            return Ok(dtos);
        }

        /// <summary>
        /// Retorna um carrapato específico pelo código serial.
        /// </summary>
        /// <param name="codigoSerial">Código serial do carrapato.</param>
        /// <returns>O carrapato encontrado, ou NotFound se não existir.</returns>
        [HttpGet("{codigoSerial}")]
        [ProducesResponseType(typeof(CarrapatoLeituraDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CarrapatoLeituraDto>> GetPorCodigoSerial(string codigoSerial)
        {
            var carrapato = await _service.ObterPorCodigoSerialAsync(codigoSerial);
            if (carrapato is null)
                return NotFound($"Carrapato com código {codigoSerial} não encontrado.");

            var dto = MapearParaDto(carrapato);
            return Ok(dto);
        }

        /// <summary>
        /// Adiciona um novo carrapato ao MongoDB.
        /// </summary>
        /// <param name="dto">Dados para criação do carrapato.</param>
        /// <returns>O carrapato criado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CarrapatoLeituraDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CarrapatoLeituraDto>> Post([FromBody] CarrapatoCriarDto dto)
        {
            var carrapato = new Carrapato(dto.CodigoSerial, dto.IdPatio);
            await _service.AdicionarAsync(carrapato);

            var leituraDto = MapearParaDto(carrapato);
            return CreatedAtAction(nameof(GetPorCodigoSerial),
                new { codigoSerial = carrapato.CodigoSerial }, leituraDto);
        }

        /// <summary>
        /// Atualiza os dados de um carrapato existente.
        /// </summary>
        /// <param name="codigoSerial">Código serial do carrapato a ser atualizado.</param>
        /// <param name="dto">Dados para atualização.</param>
        [HttpPut("{codigoSerial}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(string codigoSerial, [FromBody] CarrapatoCriarDto dto)
        {
            var carrapato = new Carrapato(dto.CodigoSerial, dto.IdPatio);
            await _service.AtualizarAsync(codigoSerial, carrapato);
            return NoContent();
        }

        /// <summary>
        /// Remove um carrapato do MongoDB pelo código serial.
        /// </summary>
        /// <param name="codigoSerial">Código serial do carrapato a ser removido.</param>
        [HttpDelete("{codigoSerial}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(string codigoSerial)
        {
            await _service.DeletarAsync(codigoSerial);
            return NoContent();
        }

        private CarrapatoLeituraDto MapearParaDto(Carrapato carrapato)
        {
            return new CarrapatoLeituraDto(
                carrapato.Id,
                carrapato.CodigoSerial,
                carrapato.StatusBateria.ToString(),
                carrapato.StatusDeUso.ToString(),
                carrapato.IdPatio
            );
        }
    }
}