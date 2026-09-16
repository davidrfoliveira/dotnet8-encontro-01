public enum SituacaoApolice
{
    EmAnalise,
    Ativa,
    Suspensa,
    Cancelada,
    Vencido
}

[Flags]
public enum TipoCobertura
{
    Roubo = 1, Incendio = 2, Colisao = 3, DanosTerceiros = 8, FenomenoNaturais, Morte = 32
}

public interface ICalculavel
{
    decimal CalcularPremio();
    
}

public class Cobertura
{
    public TipoCobertura Tipo { get; set; }
    public string Descricao { get; set; }
    public decimal LimiteIndenizacao { get; set; }
}

public abstract class Apolice : ICalculavel
{
    private readonly List<Cobertura> _coberturas = [];

    public string Numero { get; set; }
    public Segurado Segurado { get; init; } = null!;
    public decimal ValorSegurado { get; set; }
    public SituacaoApolice Situacao { get; private set; } = SituacaoApolice.EmAnalise;
    public IReadOnlyList<Cobertura> Coberturas => _coberturas;
    public abstract decimal CalcularPremio();

    //metodos

    public void AdicionarCobertura(Cobertura cobertura)
    {
        if (_coberturas.Any(p => p.Tipo == cobertura.Tipo))
        {
            throw new InvalidOperationException(
                $"A cobertura {cobertura.Tipo} já está na apólice {Numero}."
            );
        }
        _coberturas.Add(cobertura);
    }

    public void Ativar()
    {
        if (Situacao != SituacaoApolice.EmAnalise)
        {
            throw new InvalidOperationException(
                $"Só é possível ativar apólice em análise. Situação: {Situacao}"
            );
        }

        if (_coberturas.Count == 0)
        {
            throw new InvalidOperationException($"Apólice precisa de ao menos uma cobertura para ser ativada.");
        }

        if (!Segurado.MaiorDeIdade)
        {
            throw new InvalidOperationException($"Segurado precisa ser maior de idade");
        }

    }
}

public class ApoliceAutomovel : Apolice
{
    public string Placa { get; set; }
    public int AnoFabricacao { get; set; }

    public override decimal CalcularPremio()
    {
        var taxa = 0.05m;
        if(DateTime.Today.Year - AnoFabricacao > 10) taxa += 0.02m;

        if(Segurado.Idade < 25) taxa =+ 0.03m;

        return Math.Round(ValorSegurado * taxa, 1);
    }


}