using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Seguradora.Api.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("laboratorio")]
public class LaboratorioController : ControllerBase
{
    private readonly SeguradoraDbContext _contexto;

    public LaboratorioController(SeguradoraDbContext contexto)
    {
        _contexto = contexto;
    }

    [HttpGet("buscar-inseguro")]
    public async Task<IActionResult> BuscarInseguro(string nome)
    {
        var sql = "SELECT * FROM Segurados WHERE Nome LIKE '%" + nome + "%'";

        var segurados = await _contexto.Segurados
            .FromSqlRaw(sql)
            .AsNoTracking()
            .ToListAsync();

        return Ok(segurados.Select(SeguradoResponse.De));
    }

    [HttpGet("buscar-seguro")]
    public async Task<IActionResult> BuscarSeguro(string nome)
    {
        var padrao = $"%{nome}%";

        var segurados = await _contexto.Segurados
            .FromSql($"SELECT * FROM Segurados WHERE Nome LIKE {padrao}")
            .AsNoTracking()
            .ToListAsync();

        return Ok(segurados.Select(SeguradoResponse.De));
    }
}
