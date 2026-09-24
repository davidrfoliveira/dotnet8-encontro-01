using System.Globalization;
using System.Security.Claims;

public static class UsuarioLogadoExtensions
{
    public static string? SeguradoId(this ClaimsPrincipal usuario) =>
        usuario.FindFirst("segurado_id")?.Value;

    public static decimal? Alcada(this ClaimsPrincipal usuario) =>
        decimal.TryParse(usuario.FindFirst("alcada")?.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var alcada)
            ? alcada
            : null;
}
