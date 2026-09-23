public class ApoliceRepositorioEmMemoria 
{
    private readonly List<Apolice> _apolices;

    public ApoliceRepositorioEmMemoria(ISeguradoRepositorio segurados)
    {
        var maria = segurados.Listar().First(s => s.Nome == "Maria Silva");
        var joao = segurados.Listar().First(s => s.Nome == "João Souza");

        var auto = new ApoliceAutomovel
        {
            Id = "AUTO-2026-001",
            Segurado = joao,
            ValorSegurado = 85_000m,
            Inicio = new DateTime(2026, 1, 10),
            Placa = "ABC1D23",
            AnoFabricacao = 2012
        };
        auto.AdicionarCobertura(new Cobertura { Id = "COB-1", Tipo = TipoCobertura.Colisao, Descricao = "Colisão e capotagem", LimiteIndenizacao = 85_000m });
        auto.AdicionarCobertura(new Cobertura { Id = "COB-2", Tipo = TipoCobertura.Roubo, Descricao = "Roubo e furto", LimiteIndenizacao = 85_000m });

        var residencial = new ApoliceResidencial
        {
            Id = "RES-2026-002",
            Segurado = maria,
            ValorSegurado = 450_000m,
            Inicio = new DateTime(2026, 2, 1),
            AreaConstruida = 180m,
            PossuiAlarme = true
        };
        residencial.AdicionarCobertura(new Cobertura { Id = "COB-3", Tipo = TipoCobertura.Incendio, Descricao = "Incêndio e explosão", LimiteIndenizacao = 450_000m });

        var vida = new ApoliceVida
        {
            Id = "VIDA-2026-003",
            Segurado = maria,
            ValorSegurado = 200_000m,
            Inicio = new DateTime(2026, 3, 1),
            Fumante = false
        };
        vida.AdicionarCobertura(new Cobertura { Id = "COB-4", Tipo = TipoCobertura.Morte, Descricao = "Morte por qualquer causa", LimiteIndenizacao = 200_000m });

        _apolices = new List<Apolice> { auto, residencial, vida };
    }

    public IReadOnlyList<Apolice> ListarAsync() =>  _apolices;
    public Apolice? ObterPorIdAsync(string id) => _apolices.FirstOrDefault(a => a.Id == id);
}
