using System.Net.Http.Headers;

public class Segurado
{
    public string Nome { get; init; } = string.Empty;
    public Cpf Documento { get; init; }
    public DateTime DataNascimento { get; init; }
    public DateTime DataCadastro { get; init; } = DateTime.Now;


    public int Idade
    {
        get
        {
            var idade = DateTime.Today.Year - DataNascimento.Year;
            // Se o aniversário deste ano ainda não chegou, tira um.
            if (DateTime.Today < DataNascimento.AddYears(idade))
                idade--;
            return idade;
        }
    }
    public bool MaiorDeIdade => Idade >= 18;
}