
var maria = new Segurado
{
    Nome = "Maria Silva",
    Documento = new Cpf("529.982.247-25"),
    DataNascimento = new DateTime(1988, 4, 12)
};


Console.WriteLine($"{maria.Nome}, {maria.Idade} anos");
Console.WriteLine($"Maior de idade: {maria.MaiorDeIdade}");


//2
var apolice = new ApoliceAutomovel
{
Numero = "AUTO-001", Segurado = maria, ValorSegurado = 85_000m,
Placa = "ABC1D23", AnoFabricacao = 2012
};

apolice.AdicionarCobertura(new Cobertura
{ 
    Tipo = TipoCobertura.Colisao, 
    Descricao = "Colisão", 
    LimiteIndenizacao = 85_000m 
});
apolice.Ativar();
Console.WriteLine($"{apolice.Situacao}, prêmio {apolice.CalcularPremio():C}");