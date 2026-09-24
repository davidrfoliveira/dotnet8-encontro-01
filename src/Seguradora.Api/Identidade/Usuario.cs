using Microsoft.AspNetCore.Identity;
public class Usuario : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;
    public string? SeguradoId { get; set; }
    public decimal? Alcada { get; set; }
}