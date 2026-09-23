using Microsoft.EntityFrameworkCore;

public class ApoliceEntityFrameworkRepositorio : IApoliceRepositorio
{
    private readonly SeguradoraDbContext _contexto;
    public ApoliceEntityFrameworkRepositorio(SeguradoraDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<IReadOnlyList<Apolice>> ListarAsync() => await _contexto.Apolices
        .AsNoTracking()
        .Include(a => a.Segurado)
        .Include(a => a.Coberturas)
        .OrderBy(a => a.Id)
        .ToListAsync();

    public async Task<Apolice?> ObterPorIdAsync(string id) =>
       await _contexto.Apolices
       .Include(a => a.Segurado)
       .Include(a => a.Coberturas)
       .FirstOrDefaultAsync(a => a.Id == id);

    public async Task SalvarAlteracoesAsync() => await _contexto.SaveChangesAsync();

}