public enum SituacaoApolice { EmAnalise, Ativa, Suspensa, Cancelada, Vencida }

[Flags]
public enum TipoCobertura
{
    Roubo = 1, Incendio = 2, Colisao = 4,
    DanosTerceiros = 8, FenomenosNaturais = 16, Morte = 32
}

public interface ICalculavel
{
    decimal CalcularPremio();
}
public class Cobertura
{
    public TipoCobertura Tipo { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public decimal LimiteIndenizacao { get; init; }
}

public abstract class Apolice : ICalculavel
{
    private readonly List<Cobertura> _coberturas = [];
    public string Numero { get; init; } = string.Empty;
    public Segurado Segurado { get; init; } = null!;
    public decimal ValorSegurado { get; init; }
    public SituacaoApolice Situacao { get; private set; } = SituacaoApolice.EmAnalise;
    public IReadOnlyList<Cobertura> Coberturas => _coberturas;
    public abstract decimal CalcularPremio();

    public void AdicionarCobertura(Cobertura cobertura)
    {
        if (_coberturas.Any(c => c.Tipo == cobertura.Tipo))
            throw new InvalidOperationException(
            $"A cobertura {cobertura.Tipo} já está na apólice {Numero}.");
        _coberturas.Add(cobertura);
    }

    public void Ativar()
    {
        if (Situacao != SituacaoApolice.EmAnalise)
            throw new InvalidOperationException(
            $"Só é possível ativar apólice em análise. Situação: {Situacao}.");
        if (_coberturas.Count == 0)
            throw new InvalidOperationException(
            "Apólice precisa de ao menos uma cobertura para ser ativada.");
        if (!Segurado.MaiorDeIdade)
            throw new InvalidOperationException("Segurado precisa ser maior de idade.");
        Situacao = SituacaoApolice.Ativa;
    }

}

public class ApoliceAutomovel : Apolice
{
    public string Placa { get; init; } = string.Empty;
    public int AnoFabricacao { get; init; }
    public override decimal CalcularPremio()
    {
        var taxa = 0.05m;
        if (DateTime.Today.Year - AnoFabricacao > 10) taxa += 0.02m;
        if (Segurado.Idade < 25) taxa += 0.03m;
        return Math.Round(ValorSegurado * taxa, 2);
    }
}