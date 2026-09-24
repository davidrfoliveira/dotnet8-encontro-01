public interface IApoliceRepositorio
{
    Task<IReadOnlyList<Apolice>> ListarAsync(string? seguradoId = null);
    Task<Apolice?> ObterPorIdAsync(string id);
    Task SalvarAlteracoesAsync();
}
