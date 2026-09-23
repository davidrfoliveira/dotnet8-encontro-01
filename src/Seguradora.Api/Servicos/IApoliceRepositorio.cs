public interface IApoliceRepositorio
{
    Task<IReadOnlyList<Apolice>> ListarAsync();
    Task<Apolice?> ObterPorIdAsync(string id);
    Task SalvarAlteracoesAsync();
}