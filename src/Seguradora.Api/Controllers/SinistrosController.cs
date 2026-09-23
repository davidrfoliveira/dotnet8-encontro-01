using Microsoft.AspNetCore.Mvc;
namespace Seguradora.Api.Controllers;

[ApiController]
[Route("sinistros")]
public class SinistrosController : ControllerBase
{
    private readonly ISinistroRepositorio _sinistros;
    private readonly IApoliceRepositorio _apolices;
    public SinistrosController(ISinistroRepositorio sinistros, IApoliceRepositorio apolices)
    {
        _sinistros = sinistros;
        _apolices = apolices;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SinistroResponse>> ObterPorId(string id)
    {
        var sinistro = await _sinistros.ObterPorIdAsync(id);
        return sinistro is null ? NotFound() : Ok(SinistroResponse.De(sinistro));
    }

    [HttpPost]
    public async Task<ActionResult<SinistroResponse>> Abrir(SinistroRequest requisicao)
    {
        var apolice = await _apolices.ObterPorIdAsync(requisicao.ApoliceId);
        if (apolice is null) return NotFound();
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

    [HttpPost("{id}/analisar")]
    public async Task<ActionResult<SinistroResponse>> Analisar(string id)
    {
        var sinistro = await _sinistros.ObterPorIdAsync(id);
        if (sinistro is null) return NotFound();
        sinistro.Analisar();
        await _sinistros.SalvarAlteracoesAsync();
        return Ok(SinistroResponse.De(sinistro));
    }

}