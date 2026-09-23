public interface ISinistroRepositorio
{
    Task<Sinistro?> ObterPorIdAsync(string id);
    Task AdicionarAsync(Sinistro sinistro);
    Task SalvarAlteracoesAsync();
}