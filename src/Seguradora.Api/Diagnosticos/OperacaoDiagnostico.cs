// Código de apoio do Bloco 1 (Encontro 3): demonstra na prática a diferença entre
// Transient, Scoped e Singleton. Não faz parte do domínio da Seguradora.

namespace Seguradora.Api.Diagnostico;

public interface IOperacaoTransient
{
    Guid OperacaoId { get; }
}

public interface IOperacaoScoped
{
    Guid OperacaoId { get; }
}

public interface IOperacaoSingleton
{
    Guid OperacaoId { get; }
}

public class OperacaoDiagnostico : IOperacaoTransient, IOperacaoScoped, IOperacaoSingleton
{
    public Guid OperacaoId { get; } = Guid.NewGuid();
}

public interface IServicoDiagnostico
{
    Guid TransientId { get; }
    Guid ScopedId { get; }
    Guid SingletonId { get; }
}

// Recebe as três dependências pelo construtor — o próprio host resolve cada uma
// com o tempo de vida certo, sem este serviço saber como elas são montadas.
public class ServicoDiagnostico : IServicoDiagnostico
{
    public ServicoDiagnostico(
        IOperacaoTransient transient,
        IOperacaoScoped scoped,
        IOperacaoSingleton singleton)
    {
        TransientId = transient.OperacaoId;
        ScopedId = scoped.OperacaoId;
        SingletonId = singleton.OperacaoId;
    }

    public Guid TransientId { get; }
    public Guid ScopedId { get; }
    public Guid SingletonId { get; }
}
