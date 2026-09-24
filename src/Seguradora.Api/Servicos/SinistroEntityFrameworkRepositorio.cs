using Microsoft.EntityFrameworkCore;

public class SinistroEntityFrameworkRepositorio : ISinistroRepositorio
{
    private readonly SeguradoraDbContext _contexto;

    public SinistroEntityFrameworkRepositorio(SeguradoraDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IReadOnlyList<Sinistro>> ListarAsync(
        string? seguradoId = null,
        SituacaoSinistro? situacao = null,
        decimal? valorMaximo = null)
    {
        IQueryable<Sinistro> consulta = _contexto.Sinistros
            .AsNoTracking()
            .Include(s => s.Apolice).ThenInclude(a => a.Coberturas);

        if (seguradoId is not null)
            consulta = consulta.Where(s => s.Apolice.Segurado.Id == seguradoId);

        if (situacao is not null)
            consulta = consulta.Where(s => s.Situacao == situacao);

        if (valorMaximo is not null)
            consulta = consulta.Where(s => s.ValorPleiteado <= valorMaximo);

        return await consulta.OrderBy(s => s.Id).ToListAsync();
    }

    public async Task<Sinistro?> ObterPorIdAsync(string id) =>
        await _contexto.Sinistros
            .Include(s => s.Apolice).ThenInclude(a => a.Segurado)
            .Include(s => s.Apolice).ThenInclude(a => a.Coberturas)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task AdicionarAsync(Sinistro sinistro)
    {
        _contexto.Sinistros.Add(sinistro);
        await _contexto.SaveChangesAsync();
    }

    public async Task SalvarAlteracoesAsync() =>
        await _contexto.SaveChangesAsync();
}
