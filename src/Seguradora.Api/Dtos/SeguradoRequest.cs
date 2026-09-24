using System.ComponentModel.DataAnnotations;

public record SeguradoRequest(
    [Required, StringLength(120, MinimumLength = 3)] string Nome,
    [Required, StringLength(14, MinimumLength = 11)] string Cpf,
    DateTime DataNascimento);
