public interface ISeguradoRepositorio
{
      IReadOnlyList<Segurado> Listar();
    Segurado? ObterPorId(string id);
    void Adicionar(Segurado segurado);
}