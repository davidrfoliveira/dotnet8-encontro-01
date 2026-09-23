public record SinistroRequest(
    string ApoliceId,
    TipoCobertura TipoOcorrencia,
    DateTime DataOcorrencia,
    string Descricao,
    decimal ValorPleiteado);