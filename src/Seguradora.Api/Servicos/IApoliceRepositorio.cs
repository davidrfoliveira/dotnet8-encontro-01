public interface IApoliceRepositorio
{
    IReadOnlyList<Apolice> Listar();
    Apolice? ObterPorId(string id);
}