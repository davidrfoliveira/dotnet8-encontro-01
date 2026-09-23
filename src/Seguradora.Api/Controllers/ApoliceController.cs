using Microsoft.AspNetCore.Mvc;
namespace Seguradora.Api.Controllers;
[ApiController]
[Route("apolices")]
public class ApolicesController : ControllerBase
{
    private readonly IApoliceRepositorio _repo;
    public ApolicesController(IApoliceRepositorio repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Apolice>>> Listar() =>Ok(await _repo.ListarAsync());
    [HttpGet("{id}")]
    public async Task<ActionResult<Apolice>> ObterPorId(string id)
    {
        var apolice = await _repo.ObterPorIdAsync(id);
        return apolice is null ? NotFound() : Ok(apolice);
    }
    [HttpPost("{id}/ativar")]
    public async Task<ActionResult<Apolice>> Ativar(string id)
    {
        var apolice = await _repo.ObterPorIdAsync(id);
        if (apolice is null) return NotFound();
        apolice.Ativar();
        await _repo.SalvarAlteracoesAsync();
        return Ok(apolice);
    }
}