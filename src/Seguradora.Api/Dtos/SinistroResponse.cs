public record SinistroResponse(string Id,
string ApoliceId,
TipoCobertura TipoOcorrencia,
DateTime DataOcorrencia,
string Descricao,
decimal ValorPleiteado,
SituacaoSinistro Situacao,
string? MotivoNegativa,
decimal Indenizacao)
{
    public static SinistroResponse De(Sinistro s) => new(
        s.Id,
        s.Apolice.Id,
        s.TipoOcorrencia,
        s.DataOcorrencia,
        s.Descricao,
        s.ValorPleiteado,
        s.Situacao,
        s.MotivoNegativa,
        s.CalcularIndenizacao());
}