
var maria = new Segurado
{
    Nome = "Maria Silva",
    Documento = new Cpf("529.982.247-25"),
    DataNascimento = new DateTime(1988, 4, 12)
};

var apolice = new ApoliceAutomovel
{
    Numero = "AUTO-001", 
    Segurado = maria,
    ValorSegurado = 85000,
    Placa = "ABC1D26",
    AnoFabricacao = 2012
};

apolice.AdicionarCobertura(new Cobertura
{
    Tipo = TipoCobertura.Colisao,
    Descricao = "Colisão",
    LimiteIndenizacao = 85000
});

apolice.Ativar();

Console.WriteLine($"{apolice.Situacao}, prêmio {apolice.CalcularPremio():C}");