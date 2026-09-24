public interface ISinistroRepositorio
{
    Task<IReadOnlyList<Sinistro>> ListarAsync(
        string? seguradoId = null,
        SituacaoSinistro? situacao = null,
        decimal? valorMaximo = null);
    Task<Sinistro?> ObterPorIdAsync(string id);
    Task AdicionarAsync(Sinistro sinistro);
    Task SalvarAlteracoesAsync();
}
