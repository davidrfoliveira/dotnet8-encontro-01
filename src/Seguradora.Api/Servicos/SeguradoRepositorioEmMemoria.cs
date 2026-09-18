public class SeguradoRepositorioEmMemoria : ISeguradoRepositorio
{
      private readonly List<Segurado> _segurados = new()
    {
              new() { Nome = "Maria Silva", Documento = new Cpf("529.982.247-25"), DataNascimento = new DateTime(1988, 4, 12) },
              new() { Nome = "João Souza", Documento = new Cpf("11144477735"), DataNascimento = new DateTime(2004, 9, 30) }
    };
      public IReadOnlyList<Segurado> Listar() => _segurados;

    public Segurado? ObterPorId(string id) => _segurados.FirstOrDefault(p => p.Documento.Numero == id);
    
}
