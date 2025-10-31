using Aplicacao.DTOs.Carrapato;
using Aplicacao.Servicos;
using Dominio.Persistencia;
using FluentValidation;
using FluentValidation.Results;
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
        private readonly IValidator<CarrapatoCriarDto> _validator;

        public CarrapatoMongoControlador(CarrapatoMongoService service, IValidator<CarrapatoCriarDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<CarrapatoMongoLeituraDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<CarrapatoMongoLeituraDto>>> GetTodos()
        {
            var carrapatos = await _service.ObterTodosAsync();
            var dtos = carrapatos.Select(MapearParaDto).ToList();
            return Ok(dtos);
        }

        [HttpGet("{codigoSerial}")]
        [ProducesResponseType(typeof(CarrapatoMongoLeituraDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CarrapatoMongoLeituraDto>> GetPorCodigoSerial(string codigoSerial)
        {
            var carrapato = await _service.ObterPorCodigoSerialAsync(codigoSerial);
            if (carrapato is null)
                return NotFound($"Carrapato com código {codigoSerial} não encontrado.");

            var dto = MapearParaDto(carrapato);
            return Ok(dto);
        }

        /// <remarks>
        /// Exemplo de payload:
        /// <example>
        /// {
        ///   "CodigoSerial": "CAR-0001-XYZ",
        ///   "IdPatio": 1
        /// }
        /// </example>
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(CarrapatoMongoLeituraDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CarrapatoMongoLeituraDto>> Post([FromBody] CarrapatoCriarDto dto)
        {
            ValidationResult result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            var carrapato = new CarrapatoMongo(dto.CodigoSerial, dto.IdPatio);
            var criado = await _service.AdicionarAsync(carrapato);

            var leituraDto = MapearParaDto(criado);
            return CreatedAtAction(nameof(GetPorCodigoSerial),
                new { codigoSerial = criado.CodigoSerial }, leituraDto);
        }

        [HttpPut("{codigoSerial}")]
        [ProducesResponseType(typeof(CarrapatoMongoLeituraDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CarrapatoMongoLeituraDto>> Put(string codigoSerial, [FromBody] CarrapatoCriarDto dto)
        {
            ValidationResult result = await _validator.ValidateAsync(dto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
            }

            var carrapato = new CarrapatoMongo(dto.CodigoSerial, dto.IdPatio);
            var atualizado = await _service.AtualizarAsync(codigoSerial, carrapato);

            if (atualizado is null)
                return NotFound($"Carrapato com código {codigoSerial} não encontrado.");

            var leituraDto = MapearParaDto(atualizado);
            return Ok(leituraDto);
        }

        [HttpDelete("{codigoSerial}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> Delete(string codigoSerial)
        {
            var existente = await _service.ObterPorCodigoSerialAsync(codigoSerial);
            if (existente is null)
                return NotFound($"Carrapato com código {codigoSerial} não encontrado.");

            await _service.DeletarAsync(codigoSerial);

            return Ok(new { mensagem = $"Carrapato {codigoSerial} excluído com sucesso." });
        }

        private CarrapatoMongoLeituraDto MapearParaDto(CarrapatoMongo carrapato)
        {
            return new CarrapatoMongoLeituraDto(
                carrapato.Id,
                carrapato.CodigoSerial,
                carrapato.StatusBateria.ToString(),
                carrapato.StatusDeUso.ToString(),
                carrapato.IdPatio
            );
        }
    }
}