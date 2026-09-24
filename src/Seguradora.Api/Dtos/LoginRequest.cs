using System.ComponentModel.DataAnnotations;

public record LoginRequest(
    [Required, EmailAddress, StringLength(254)] string Email,
    [Required, StringLength(100)] string Senha);
