public class RefreshToken
{
    public int Id { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime ExpiraEm { get; set; }
    public DateTime? RevogadoEm { get; set; }
    public string? SubstituidoPorHash { get; set; }
}
