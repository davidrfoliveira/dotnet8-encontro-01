using System.ComponentModel.DataAnnotations;

public record SinistroRequest(
    [Required, StringLength(20)] string ApoliceId,
    TipoCobertura TipoOcorrencia,
    DateTime DataOcorrencia,
    [Required, StringLength(500, MinimumLength = 5)] string Descricao,
    [Range(0.01, 10_000_000)] decimal ValorPleiteado);
