
var maria = new Segurado
{
    Nome = "Maria Silva",
    Documento = new Cpf("529.982.247-25"),
    DataNascimento = new DateTime(1988, 4, 12)
};


Console.WriteLine($"{maria.Nome}, {maria.Idade} anos");
Console.WriteLine($"Maior de idade: {maria.MaiorDeIdade}");
