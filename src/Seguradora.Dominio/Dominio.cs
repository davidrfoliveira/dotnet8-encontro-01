// ============================================================================
// ENCONTRO 1 —  EXERCÍCIO
// Domínio inicial da API de Gestão de Apólices e Sinistros
// ----------------------------------------------------------------------------
// Esta é uma das soluções possíveis. O objetivo do exercício não é chegar
// exatamente neste código, e sim exercitar propriedades, interfaces, herança,
// enum e validação. Compare com o seu e discuta as diferenças.
// ============================================================================



// ===========================================================================
// ENUMS
// ===========================================================================

public enum SituacaoApolice
{
    EmAnalise = 1,
    Ativa = 2,
    Suspensa = 3,
    Cancelada = 4,
    Vencida = 5
}

public enum SituacaoSinistro
{
    Aberto = 1,
    EmRegulacao = 2,
    Aprovado = 3,
    Negado = 4,
    Pago = 5
}

[Flags]
public enum TipoCobertura
{
    Nenhuma = 0,
    Roubo = 1,
    Incendio = 2,
    Colisao = 4,
    DanosTerceiros = 8,
    FenomenosNaturais = 16,
    Morte = 32
}

// ===========================================================================
// CONTRATOS
// ===========================================================================

public interface IIdentificavel
{
    string Id { get; }
}

public interface ICalculavel
{
    decimal CalcularPremio();
}

// ===========================================================================
// VALUE OBJECT
// ===========================================================================

// struct porque é pequeno, imutável e representa um valor único.
public readonly struct Cpf
{
    public string Numero { get; }

    public Cpf(string numero)
    {
        var digitos = new string((numero ?? string.Empty).Where(char.IsDigit).ToArray());

        if (digitos.Length != 11)
            throw new ArgumentException("CPF deve conter 11 dígitos.", nameof(numero));

        if (digitos.Distinct().Count() == 1)
            throw new ArgumentException("CPF com todos os dígitos iguais é inválido.", nameof(numero));

        Numero = digitos;
    }

    public string Formatado => $"{Numero[..3]}.{Numero[3..6]}.{Numero[6..9]}-{Numero[9..]}";

    public string Mascarado => $"***.{Numero[3..6]}.{Numero[6..9]}-**";

    public override string ToString() => Formatado;
}

// ===========================================================================
// ENTIDADES
// ===========================================================================

public class Segurado : IIdentificavel
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
    public string Nome { get; init; } = string.Empty;
    public Cpf Documento { get; init; }
    public DateTime DataNascimento { get; init; }
    public DateTime DataCadastro { get; init; } = DateTime.Now;

    public int Idade
    {
        get
        {
            var idade = DateTime.Today.Year - DataNascimento.Year;
            if (DateTime.Today < DataNascimento.AddYears(idade)) idade--;
            return idade;
        }
    }

    public bool MaiorDeIdade => Idade >= 18;

    public override string ToString() => $"{Nome} ({Documento.Mascarado}), {Idade} anos";
}

public class Cobertura : IIdentificavel
{
    public string Id { get; init; } = string.Empty;
    public TipoCobertura Tipo { get; init; }
    public string Descricao { get; init; } = string.Empty;

    private decimal _limiteIndenizacao;
    public decimal LimiteIndenizacao
    {
        get => _limiteIndenizacao;
        init
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Limite de indenização deve ser maior que zero.");
            _limiteIndenizacao = value;
        }
    }

    public override string ToString() => $"{Descricao} até {LimiteIndenizacao:C}";
}

// ---------------------------------------------------------------------------
// Classe base abstrata das apólices
// ---------------------------------------------------------------------------
public abstract class Apolice : IIdentificavel, ICalculavel
{
    private readonly List<Cobertura> _coberturas = new();

    public string Id { get; init; } = string.Empty;
    public Segurado Segurado { get; init; } = null!;
    public DateTime Inicio { get; init; }
    public int VigenciaEmMeses { get; init; } = 12;
    public SituacaoApolice Situacao { get; private set; } = SituacaoApolice.EmAnalise;

    private decimal _valorSegurado;
    public decimal ValorSegurado
    {
        get => _valorSegurado;
        init
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value),
                    "Valor segurado deve ser maior que zero.");
            _valorSegurado = value;
        }
    }

    // Expõe a coleção sem permitir alteração de fora
    public IReadOnlyList<Cobertura> Coberturas => _coberturas;

    public DateTime Vencimento => Inicio.AddMonths(VigenciaEmMeses);

    public bool EstaVigente()
        => Situacao == SituacaoApolice.Ativa
           && DateTime.Today >= Inicio
           && DateTime.Today <= Vencimento;

    // abstract: cada tipo de apólice calcula do seu jeito
    public abstract decimal CalcularPremio();

    // virtual: a filha pode complementar
    public virtual string Descrever()
        => $"Apólice {Id} de {Segurado.Nome}, cobertura de {ValorSegurado:C}, " +
           $"vigência de {Inicio:dd/MM/yyyy} a {Vencimento:dd/MM/yyyy}";

    public void AdicionarCobertura(Cobertura cobertura)
    {
        ArgumentNullException.ThrowIfNull(cobertura);

        if (_coberturas.Any(c => c.Tipo == cobertura.Tipo))
            throw new InvalidOperationException(
                $"A cobertura {cobertura.Tipo} já está na apólice {Id}.");

        _coberturas.Add(cobertura);
    }

    public void Ativar()
    {
        if (Situacao != SituacaoApolice.EmAnalise)
            throw new InvalidOperationException(
                $"Só é possível ativar apólice em análise. Situação atual: {Situacao}.");

        if (_coberturas.Count == 0)
            throw new InvalidOperationException(
                "Apólice precisa de ao menos uma cobertura para ser ativada.");

        if (!Segurado.MaiorDeIdade)
            throw new InvalidOperationException(
                "Segurado precisa ser maior de idade.");

        Situacao = SituacaoApolice.Ativa;
    }

    public void Cancelar() => Situacao = SituacaoApolice.Cancelada;
}

// ---------------------------------------------------------------------------
// Especializações
// ---------------------------------------------------------------------------
public class ApoliceAutomovel : Apolice
{
    public string Placa { get; init; } = string.Empty;
    public int AnoFabricacao { get; init; }

    public int IdadeVeiculo => DateTime.Today.Year - AnoFabricacao;

    public override decimal CalcularPremio()
    {
        var taxaBase = 0.05m;
        var acrescimoIdade = IdadeVeiculo > 10 ? 0.02m : 0m;
        var acrescimoJovem = Segurado.Idade < 25 ? 0.03m : 0m;

        return ValorSegurado * (taxaBase + acrescimoIdade + acrescimoJovem);
    }

    public override string Descrever()
        => $"{base.Descrever()} | Veículo {Placa}, ano {AnoFabricacao}";
}

public class ApoliceResidencial : Apolice
{
    public decimal AreaConstruida { get; init; }
    public bool PossuiAlarme { get; init; }

    public override decimal CalcularPremio()
    {
        var premio = ValorSegurado * 0.03m + AreaConstruida * 2m;
        return PossuiAlarme ? premio * 0.9m : premio;
    }

    public override string Descrever()
        => $"{base.Descrever()} | {AreaConstruida} m²" +
           (PossuiAlarme ? " com alarme" : string.Empty);
}

public sealed class ApoliceVida : Apolice
{
    public bool Fumante { get; init; }

    public override decimal CalcularPremio()
    {
        var taxa = 0.04m;
        if (Fumante) taxa += 0.04m;
        if (Segurado.Idade > 60) taxa += 0.03m;

        return ValorSegurado * taxa;
    }
}

// ---------------------------------------------------------------------------
// Sinistro
// ---------------------------------------------------------------------------
public class Sinistro : IIdentificavel
{
    public string Id { get; init; } = string.Empty;
    public Apolice Apolice { get; init; } = null!;
    public TipoCobertura TipoOcorrencia { get; init; }
    public DateTime DataOcorrencia { get; init; }
    public string Descricao { get; init; } = string.Empty;
    public decimal ValorPleiteado { get; init; }
    public SituacaoSinistro Situacao { get; private set; } = SituacaoSinistro.Aberto;
    public string? MotivoNegativa { get; private set; }

    public bool TemCoberturaAplicavel
        => Apolice.Coberturas.Any(c => c.Tipo == TipoOcorrencia);

    public decimal CalcularIndenizacao()
    {
        var cobertura = Apolice.Coberturas.FirstOrDefault(c => c.Tipo == TipoOcorrencia);

        if (cobertura is null) return 0m;

        return Math.Min(ValorPleiteado, cobertura.LimiteIndenizacao);
    }

    public void Analisar()
    {
        if (Situacao != SituacaoSinistro.Aberto)
            throw new InvalidOperationException(
                $"Sinistro {Id} não está aberto. Situação: {Situacao}.");

        Situacao = SituacaoSinistro.EmRegulacao;

        if (!Apolice.EstaVigente())
        {
            Negar("Apólice não estava vigente na data da análise.");
            return;
        }

        if (!TemCoberturaAplicavel)
        {
            Negar($"A apólice não possui cobertura para {TipoOcorrencia}.");
            return;
        }

        if (DataOcorrencia < Apolice.Inicio || DataOcorrencia > Apolice.Vencimento)
        {
            Negar("Ocorrência fora do período de vigência da apólice.");
            return;
        }

        Situacao = SituacaoSinistro.Aprovado;
    }

    private void Negar(string motivo)
    {
        Situacao = SituacaoSinistro.Negado;
        MotivoNegativa = motivo;
    }

    public override string ToString()
    {
        var texto = $"Sinistro {Id} ({TipoOcorrencia}) em {DataOcorrencia:dd/MM/yyyy}: {Situacao}";
        return MotivoNegativa is null ? texto : $"{texto} - {MotivoNegativa}";
    }
}
