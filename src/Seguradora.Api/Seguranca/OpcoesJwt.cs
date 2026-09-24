public class OpcoesJwt
{
    public const string Secao = "Jwt";
    public string Emissor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public string Chave { get; set; } = string.Empty;
    public int MinutosDeExpiracao { get; set; } = 15;
    public int DiasDoRefreshToken { get; set; } = 7;
}