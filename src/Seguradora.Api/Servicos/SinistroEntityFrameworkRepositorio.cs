using Microsoft.EntityFrameworkCore;
public class SinistroEntityFrameworkRepositorio : ISinistroRepositorio
{
    private readonly SeguradoraDbContext _contexto;
    public SinistroEntityFrameworkRepositorio(SeguradoraDbContext contexto)
    {
        _contexto = contexto;
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