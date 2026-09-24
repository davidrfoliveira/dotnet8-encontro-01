using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Seguradora.Api.Controllers;

[Authorize]
[ApiController]
[Route("sinistros")]
public class SinistrosController : ControllerBase
{
    private readonly ISinistroRepositorio _sinistros;
    private readonly IApoliceRepositorio _apolices;
    private readonly IAuthorizationService _autorizacao;
    private readonly ILogger<SinistrosController> _logger;

    public SinistrosController(
        ISinistroRepositorio sinistros,
        IApoliceRepositorio apolices,
        IAuthorizationService autorizacao,
        ILogger<SinistrosController> logger)
    {
        _sinistros = sinistros;
        _apolices = apolices;
        _autorizacao = autorizacao;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SinistroResponse>>> Listar()
    {
        string? seguradoId = null;
        SituacaoSinistro? situacao = null;
        decimal? valorMaximo = null;

        if (User.IsInRole("Segurado"))
        {
            seguradoId = User.SeguradoId() ?? string.Empty;
        }
        else if (User.IsInRole("Regulador") && !User.IsInRole("Admin"))
        {
            situacao = SituacaoSinistro.Aberto;
            valorMaximo = User.Alcada() ?? 0m;
        }

        var sinistros = await _sinistros.ListarAsync(seguradoId, situacao, valorMaximo);
        return Ok(sinistros.Select(SinistroResponse.De));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SinistroResponse>> ObterPorId(string id)
    {
        var sinistro = await _sinistros.ObterPorIdAsync(id);
        if (sinistro is null) return NotFound();

        var acesso = await _autorizacao.AuthorizeAsync(User, sinistro.Apolice, "AcessoAoSegurado");
        if (!acesso.Succeeded) return NotFound();

        return Ok(SinistroResponse.De(sinistro));
    }

    [HttpPost]
    public async Task<ActionResult<SinistroResponse>> Abrir(SinistroRequest requisicao)
    {
        var apolice = await _apolices.ObterPorIdAsync(requisicao.ApoliceId);
        if (apolice is null) return NotFound();

        var acesso = await _autorizacao.AuthorizeAsync(User, apolice, "AcessoAoSegurado");
        if (!acesso.Succeeded) return NotFound();

        var sinistro = new Sinistro
        {
            Id = $"SIN-{DateTime.Now.Year}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            Apolice = apolice,
            TipoOcorrencia = requisicao.TipoOcorrencia,
            DataOcorrencia = requisicao.DataOcorrencia,
            Descricao = requisicao.Descricao,
            ValorPleiteado = requisicao.ValorPleiteado
        };
        await _sinistros.AdicionarAsync(sinistro);

        return CreatedAtAction(nameof(ObterPorId), new { id = sinistro.Id }, SinistroResponse.De(sinistro));
    }

    [Authorize(Policy = "AnalistaDeSinistros")]
    [HttpPost("{id}/analisar")]
    public async Task<ActionResult<SinistroResponse>> Analisar(string id)
    {
        var sinistro = await _sinistros.ObterPorIdAsync(id);
        if (sinistro is null) return NotFound();

        var alcada = await _autorizacao.AuthorizeAsync(User, sinistro, "AlcadaSuficiente");
        if (!alcada.Succeeded)
        {
            _logger.LogWarning(
                "Análise negada por alçada: usuário {UsuarioId}, sinistro {SinistroId}",
                User.FindFirst("sub")?.Value,
                sinistro.Id);

            return StatusCode(StatusCodes.Status403Forbidden, new { erro = "Alçada insuficiente para o valor deste sinistro." });
        }

        sinistro.Analisar();
        await _sinistros.SalvarAlteracoesAsync();
        return Ok(SinistroResponse.De(sinistro));
    }
}
