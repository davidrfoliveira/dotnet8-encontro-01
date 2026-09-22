using Microsoft.EntityFrameworkCore;

public class SeguradoEntityFrameworkRepositorio :  ISeguradoRepositorio
{
    private readonly SeguradoraDbContext _contexto;

    public SeguradoEntityFrameworkRepositorio(SeguradoraDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<Segurado> Listar() =>_contexto.Segurados.AsNoTracking().ToList();

    public Segurado? ObterPorId(string id) => _contexto.Segurados.Find(id);

    public void Adicionar(Segurado segurado)
    {
        _contexto.Segurados.Add(segurado);
        _contexto.SaveChanges();
    }
}