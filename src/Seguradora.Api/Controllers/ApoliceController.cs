using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Seguradora.Api.Controllers;

[Authorize]
[ApiController]
[Route("apolices")]
public class ApolicesController : ControllerBase
{
    private readonly IApoliceRepositorio _repo;
    private readonly IAuthorizationService _autorizacao;

    public ApolicesController(IApoliceRepositorio repo, IAuthorizationService autorizacao)
    {
        _repo = repo;
        _autorizacao = autorizacao;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApoliceResponse>>> Listar()
    {
        var seguradoId = User.IsInRole("Segurado")
            ? User.FindFirst("segurado_id")?.Value ?? string.Empty
            : null;

        var apolices = await _repo.ListarAsync(seguradoId);
        return Ok(apolices.Select(ApoliceResponse.De));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApoliceResponse>> ObterPorId(string id)
    {
        var apolice = await _repo.ObterPorIdAsync(id);
        if (apolice is null) return NotFound();

        var acesso = await _autorizacao.AuthorizeAsync(User, apolice, "AcessoAoSegurado");
        if (!acesso.Succeeded) return NotFound();

        return Ok(ApoliceResponse.De(apolice));
    }

    [Authorize(Roles = "Corretor,Admin")]
    [HttpPost("{id}/ativar")]
    public async Task<ActionResult<ApoliceResponse>> Ativar(string id)
    {
        var apolice = await _repo.ObterPorIdAsync(id);
        if (apolice is null) return NotFound();

        apolice.Ativar();
        await _repo.SalvarAlteracoesAsync();
        return Ok(ApoliceResponse.De(apolice));
    }
}
