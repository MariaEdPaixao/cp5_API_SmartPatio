using Aplicacao.Abstracoes;
using Aplicacao.DTOs.Moto;
using Aplicacao.Servicos;
using Dominio.Excecao;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controladores;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/motos")]
[Tags("Motos")]
public class MotosControlador : ControllerBase
{
    private readonly MotoServico _motoServico;
    private readonly ILogger<MotosControlador> _logger;

    public MotosControlador(MotoServico motoServico, ILogger<MotosControlador> logger)
    {
        _motoServico = motoServico;
        _logger = logger;
    }

    /// <summary>
    /// Obtém uma lista paginada de todas as motos cadastradas em um pátio.
    /// </summary>
    /// <param name="pagina">Número da página a ser retornada (padrão: 1).</param>
    /// <param name="tamanhoPagina">Número de itens por página (padrão: 10).</param>
    /// <returns>
    /// Retorna um objeto com a página atual, total de páginas, itens e links HATEOAS.
    /// </returns>
    [HttpGet(Name = nameof(GetAllMotos))]
    [ProducesResponseType(typeof(Recurso<IResultadoPaginado<MotoLeituraDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Recurso<IResultadoPaginado<MotoLeituraDto>>>> GetAllMotos(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanhoPagina = 10)
    {
        try
        {
            var motosPaginadas = await _motoServico.ObterTodosPaginado(pagina, tamanhoPagina);

            var recurso = new Recurso<IResultadoPaginado<MotoLeituraDto>>
            {
                Dados = motosPaginadas,
                Links = this.CriarLinksPaginados(motosPaginadas, tamanhoPagina, pagina)
            };
            
            return Ok(recurso);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Problem("Erro ao buscar todas as motos");
        }
    }

    /// <summary>
    /// Retorna uma moto específica pelo ID passado por parâmetro.
    /// </summary>
    /// <param name="id"> ID da moto a ser buscada </param>
    /// <returns>
    /// 200 OK com a moto encontrada.
    /// 404 Not Found se a moto não for encontrada.
    /// 400 Bad Request se o ID for inválido (não for um número inteiro).
    /// 500 Internal Server Error se ocorrer um erro interno no servidor.
    /// </returns>
    [HttpGet("{id}", Name = nameof(GetMoto))]
    [ProducesResponseType(typeof(Recurso<MotoLeituraDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Recurso<MotoLeituraDto>>> GetMoto(int id)
    {
        try
        {
            var moto = await _motoServico.ObterPorId(id);

            var recurso = new Recurso<MotoLeituraDto>
            {
                Dados = moto,
                Links = this.CriarLinks(moto)
            };

            return Ok(recurso);
        }
        catch (ExcecaoDominio ex)
        {
            _logger.LogError(ex, ex.Message);

            return BadRequest(ex.Message);
        }
        catch (ExcecaoEntidadeNaoEncontrada ex)
        {
            _logger.LogError(ex, ex.Message);

            return NotFound($"Nenhuma moto encontrada para o id {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Problem("Erro interno do servidor");
        }
    }

    /// <summary>
    /// Insere uma nova moto no sistema.
    /// </summary>
    /// <param name="motoDto">Objeto que representa o cadastro de uma nova moto.</param>
    /// <remarks>
    /// Exemplo de payload:
    /// <example>
    /// {
    ///   "placa": "ABC1D23",
    ///   "modelo": 4,
    ///   "nomePatio": "Ipiranga",
    ///   "chassi": "12345678901234567",
    ///   "zona": 0,
    ///   "idCarrapato": 1
    /// }
    /// </example>
    /// </remarks>
    [HttpPost(Name = nameof(CriarMoto))]
    [ProducesResponseType(typeof(Recurso<MotoLeituraDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Recurso<MotoLeituraDto>>> CriarMoto([FromBody] MotoCriarDto motoDto)
    {
        try
        {
            var moto = await _motoServico.Criar(motoDto);

            var recurso = new Recurso<MotoLeituraDto>
            {
                Dados = moto,
                Links = this.CriarLinks(moto)
            };
            
            return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, recurso);
        }
        catch (ExcecaoDominio ex)
        {
            _logger.LogError(ex, ex.Message);

            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Problem("Erro interno do servidor");
        }
    }

    /// <summary>
    /// Atualiza uma moto existente pelo ID.
    /// </summary>
    /// <param name="id">ID da moto a ser atualizada</param>
    /// <param name="dto">Dados para atualização da moto</param>
    /// <remarks>
    /// Exemplo de payload para atualização:
    /// <example>
    /// {
    ///   "placa": "ABC1D23",
    ///   "modelo": 2,
    ///   "nomePatio": "Ipiranga",
    ///   "chassi": "12345678901234567",
    ///   "zona": 1,
    ///   "idCarrapato": 2
    /// }
    /// </example>
    /// </remarks>
    [HttpPut("{id}", Name = nameof(AtualizarMoto))]
    [ProducesResponseType(typeof(Recurso<MotoLeituraDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Recurso<MotoLeituraDto>>> AtualizarMoto(int id, [FromBody] MotoAtualizarDto dto)
    {
        try
        {
            var moto = await _motoServico.Atualizar(id, dto);

            var recurso = new Recurso<MotoLeituraDto>
            {
                Dados = moto,
                Links = this.CriarLinks(moto)
            };

            return Ok(recurso);
        }
        catch (ExcecaoDominio ex)
        {
            _logger.LogError(ex, ex.Message);

            return BadRequest("Dados da requisição inválidos");
        }
        catch (ExcecaoEntidadeNaoEncontrada ex)
        {
            _logger.LogError(ex, ex.Message);

            return NotFound($"Nenhuma moto encontrada para o id {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Problem("Erro interno do servidor");
        }
    }

    /// <summary>
    ///  Retorna código 204 confirmando a exclusão da moto com o ID passado por parâmetro.
    /// </summary>
    /// <param name="id"> ID da moto a ser excluída </param>
    /// <returns>
    /// 204 No Content se a moto for excluída com sucesso
    /// 404 Not Found se a moto não for encontrada.
    /// 400 Bad Request se o ID for inválido (não for um número inteiro).
    /// 500 Internal Server Error se ocorrer um erro interno no servidor.
    /// </returns>
    [HttpDelete("{id}", Name = nameof(DeletarMoto))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeletarMoto(int id)
    {
        try
        {
            await _motoServico.Remover(id);
            return NoContent();
        }
        catch (ExcecaoDominio ex)
        {
            _logger.LogError(ex, ex.Message);

            return BadRequest(ex.Message);
        }
        catch (ExcecaoEntidadeNaoEncontrada ex)
        {
            _logger.LogError(ex, ex.Message);

            return NotFound($"Nenhuma moto encontrada para o id {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);

            return Problem("Erro interno do servidor");
        }
    }

    private List<Link> CriarLinks(MotoLeituraDto moto)
    {
        var links = new List<Link>
        {
            new Link { Rel = "self", Href = Url.Link(nameof(GetMoto), new { id = moto.Id }) ?? String.Empty, Method = "GET" },
            new Link { Rel = "update", Href = Url.Link(nameof(AtualizarMoto), new { id = moto.Id }) ?? String.Empty, Method = "PUT" },
            new Link { Rel = "delete", Href = Url.Link(nameof(DeletarMoto), new { id = moto.Id }) ?? String.Empty, Method = "DELETE" },
            new Link { Rel = "collection", Href = Url.Link(nameof(GetAllMotos), null) ?? String.Empty, Method = "GET" }
        };

        return links;
    }

    private List<Link> CriarLinksPaginados(
        IResultadoPaginado<MotoLeituraDto> motosPaginadas,
        int tamanho,
        int pagina)
    {
        var links = new List<Link>
        {
            new Link { Rel = "self", Href = Url.Link(nameof(GetAllMotos), new { pagina, tamanhoPagina = tamanho }) ?? String.Empty, Method = "GET" },
            new Link
            {
                Rel = "first", Href = Url.Link(nameof(GetAllMotos), new { pagina = 1, tamanhoPagina = tamanho }) ?? String.Empty, Method = "GET"
            },
        };
        
        var paginaAtual = motosPaginadas.Pagina;

        var totalPaginas = motosPaginadas.TotalPaginas;

        if (paginaAtual < totalPaginas)
        {
            links.Add(
                new Link
                {
                    Rel = "next",
                    Href = Url.Link(nameof(GetAllMotos), new { pagina = paginaAtual + 1, tamanhoPagina = tamanho }) ?? String.Empty,
                    Method = "GET"
                });
        }

        if (paginaAtual > 1)
        {
            links.Add(
                new Link
                {
                    Rel  = "prev",
                    Href = Url.Link(nameof(GetAllMotos), new { pagina = paginaAtual - 1, tamanhoPagina = tamanho }) ?? String.Empty,
                    Method = "GET"
                });
        }
        
        links.Add(
            new Link
            {
                Rel = "last",
                Href = Url.Link(nameof(GetAllMotos), new { pagina = totalPaginas, tamanhoPagina = tamanho }) ?? String.Empty,
                Method = "GET"
            });

        return links;

    }
}