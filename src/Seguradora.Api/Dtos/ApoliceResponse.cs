public record CoberturaResponse(
    string Id,
    TipoCobertura Tipo,
    string Descricao,
    decimal LimiteIndenizacao);

public record ApoliceResponse(
    string Id,
    string Tipo,
    string SeguradoNome,
    SituacaoApolice Situacao,
    decimal ValorSegurado,
    DateTime Inicio,
    DateTime Vencimento,
    decimal Premio,
    IReadOnlyList<CoberturaResponse> Coberturas)
{
    public static ApoliceResponse De(Apolice a) => new(
        a.Id,
        a.GetType().Name.Replace("Apolice", string.Empty),
        a.Segurado.Nome,
        a.Situacao,
        a.ValorSegurado,
        a.Inicio,
        a.Vencimento,
        a.CalcularPremio(),
        a.Coberturas
            .Select(c => new CoberturaResponse(c.Id, c.Tipo, c.Descricao, c.LimiteIndenizacao))
            .ToList());
}
