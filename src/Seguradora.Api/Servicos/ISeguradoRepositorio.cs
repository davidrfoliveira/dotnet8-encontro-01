public interface ISeguradoRepositorio
{
      IReadOnlyList<Segurado> Listar();
    Segurado? ObterPorId(string id);
}