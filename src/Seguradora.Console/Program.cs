
Console.WriteLine("==========================================================");
Console.WriteLine("  API de Gestão de Apólices e Sinistros — Domínio inicial");
Console.WriteLine("==========================================================\n");

// --- Segurados -------------------------------------------------------------
var maria = new Segurado
{
    Nome = "Maria Silva",
    Documento = new Cpf("529.982.247-25"),
    DataNascimento = new DateTime(1988, 4, 12)
};

var joao = new Segurado
{
    Nome = "João Souza",
    Documento = new Cpf("11144477735"),
    DataNascimento = new DateTime(2004, 9, 30)
};

Console.WriteLine("SEGURADOS");
Console.WriteLine($"  {maria}");
Console.WriteLine($"  {joao}\n");

// --- Apólices --------------------------------------------------------------
var auto = new ApoliceAutomovel
{
    Id = "AUTO-2026-001",
    Segurado = joao,
    ValorSegurado = 85_000m,
    Inicio = new DateTime(2026, 1, 10),
    Placa = "ABC1D23",
    AnoFabricacao = 2012
};

auto.AdicionarCobertura(new Cobertura
{
    Id = "COB-1", Tipo = TipoCobertura.Colisao,
    Descricao = "Colisão e capotagem", LimiteIndenizacao = 85_000m
});
auto.AdicionarCobertura(new Cobertura
{
    Id = "COB-2", Tipo = TipoCobertura.Roubo,
    Descricao = "Roubo e furto", LimiteIndenizacao = 85_000m
});

var residencial = new ApoliceResidencial
{
    Id = "RES-2026-002",
    Segurado = maria,
    ValorSegurado = 450_000m,
    Inicio = new DateTime(2026, 2, 1),
    AreaConstruida = 180m,
    PossuiAlarme = true
};

residencial.AdicionarCobertura(new Cobertura
{
    Id = "COB-3", Tipo = TipoCobertura.Incendio,
    Descricao = "Incêndio e explosão", LimiteIndenizacao = 450_000m
});

var vida = new ApoliceVida
{
    Id = "VIDA-2026-003",
    Segurado = maria,
    ValorSegurado = 200_000m,
    Inicio = new DateTime(2026, 3, 1),
    Fumante = false
};

vida.AdicionarCobertura(new Cobertura
{
    Id = "COB-4", Tipo = TipoCobertura.Morte,
    Descricao = "Morte por qualquer causa", LimiteIndenizacao = 200_000m
});

var carteira = new List<Apolice> { auto, residencial, vida };

// --- Ativação --------------------------------------------------------------
Console.WriteLine("ATIVAÇÃO");
foreach (var apolice in carteira)
{
    try
    {
        apolice.Ativar();
        Console.WriteLine($"  {apolice.Id}: {apolice.Situacao}");
    }
    catch (InvalidOperationException ex)
    {
        Console.WriteLine($"  {apolice.Id}: recusada - {ex.Message}");
    }
}

// --- Carteira --------------------------------------------------------------
Console.WriteLine("\nCARTEIRA");
foreach (var apolice in carteira)
{
    Console.WriteLine($"  {apolice.Descrever()}");
    Console.WriteLine($"    Prêmio: {apolice.CalcularPremio():C} | " +
                      $"Coberturas: {apolice.Coberturas.Count} | " +
                      $"Vigente: {apolice.EstaVigente()}");
}

Console.WriteLine($"\n  Prêmio total da carteira: {carteira.Sum(a => a.CalcularPremio()):C}");

// --- Regra duplicada -------------------------------------------------------
Console.WriteLine("\nVALIDAÇÕES");
try
{
    auto.AdicionarCobertura(new Cobertura
    {
        Id = "COB-5", Tipo = TipoCobertura.Roubo,
        Descricao = "Roubo duplicado", LimiteIndenizacao = 10_000m
    });
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  {ex.Message}");
}

try
{
    _ = new Cpf("123");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  {ex.Message}");
}

// --- Sinistros -------------------------------------------------------------
Console.WriteLine("\nSINISTROS");

var sinistros = new List<Sinistro>
{
    new()
    {
        Id = "SIN-001", Apolice = auto, TipoOcorrencia = TipoCobertura.Colisao,
        DataOcorrencia = new DateTime(2026, 5, 20),
        Descricao = "Colisão traseira em via urbana", ValorPleiteado = 12_000m
    },
    new()
    {
        Id = "SIN-002", Apolice = auto, TipoOcorrencia = TipoCobertura.FenomenosNaturais,
        DataOcorrencia = new DateTime(2026, 6, 5),
        Descricao = "Alagamento em garagem", ValorPleiteado = 30_000m
    },
    new()
    {
        Id = "SIN-003", Apolice = residencial, TipoOcorrencia = TipoCobertura.Incendio,
        DataOcorrencia = new DateTime(2026, 4, 18),
        Descricao = "Princípio de incêndio na cozinha", ValorPleiteado = 600_000m
    }
};

foreach (var sinistro in sinistros)
{
    sinistro.Analisar();
    Console.WriteLine($"  {sinistro}");

    if (sinistro.Situacao == SituacaoSinistro.Aprovado)
        Console.WriteLine($"    Indenização: {sinistro.CalcularIndenizacao():C} " +
                          $"(pleiteado {sinistro.ValorPleiteado:C})");
}

Console.WriteLine("\n==========================================================");