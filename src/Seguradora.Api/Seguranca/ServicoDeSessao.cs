using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

public interface IServicoDeSessao
{
    Task<LoginResponse> IniciarAsync(Usuario usuario);
    Task<LoginResponse?> RenovarAsync(string refreshToken);
    Task RevogarAsync(string refreshToken);
    Task RevogarTodasAsync(Usuario usuario);
}

public class ServicoDeSessao : IServicoDeSessao
{
    private readonly SeguradoraDbContext _contexto;
    private readonly UserManager<Usuario> _usuarios;
    private readonly IServicoDeToken _tokens;
    private readonly OpcoesJwt _opcoes;

    public ServicoDeSessao(
        SeguradoraDbContext contexto,
        UserManager<Usuario> usuarios,
        IServicoDeToken tokens,
        IOptions<OpcoesJwt> opcoes)
    {
        _contexto = contexto;
        _usuarios = usuarios;
        _tokens = tokens;
        _opcoes = opcoes.Value;
    }

    public async Task<LoginResponse> IniciarAsync(Usuario usuario)
    {
        var refreshToken = GerarRefreshToken();
        _contexto.RefreshTokens.Add(NovoRegistro(usuario.Id, refreshToken));
        await _contexto.SaveChangesAsync();

        return await MontarRespostaAsync(usuario, refreshToken);
    }

    public async Task<LoginResponse?> RenovarAsync(string refreshToken)
    {
        var hash = CalcularHash(refreshToken);
        var atual = await _contexto.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
        if (atual is null)
            return null;

        if (atual.RevogadoEm is not null)
        {
            // token já usado sendo apresentado de novo: possível roubo, derruba a sessão inteira
            await RevogarTodasDoUsuarioAsync(atual.UsuarioId);
            return null;
        }

        if (atual.ExpiraEm <= DateTime.UtcNow)
            return null;

        var usuario = await _usuarios.FindByIdAsync(atual.UsuarioId);
        if (usuario is null)
            return null;

        var novoRefreshToken = GerarRefreshToken();
        atual.RevogadoEm = DateTime.UtcNow;
        atual.SubstituidoPorHash = CalcularHash(novoRefreshToken);
        _contexto.RefreshTokens.Add(NovoRegistro(usuario.Id, novoRefreshToken));
        await _contexto.SaveChangesAsync();

        return await MontarRespostaAsync(usuario, novoRefreshToken);
    }

    public async Task RevogarAsync(string refreshToken)
    {
        var hash = CalcularHash(refreshToken);
        var atual = await _contexto.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash);
        if (atual is null || atual.RevogadoEm is not null)
            return;

        atual.RevogadoEm = DateTime.UtcNow;
        await _contexto.SaveChangesAsync();
    }

    public async Task RevogarTodasAsync(Usuario usuario)
    {
        await RevogarTodasDoUsuarioAsync(usuario.Id);
        await _usuarios.UpdateSecurityStampAsync(usuario);
    }

    private async Task RevogarTodasDoUsuarioAsync(string usuarioId) =>
        await _contexto.RefreshTokens
            .Where(t => t.UsuarioId == usuarioId && t.RevogadoEm == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevogadoEm, DateTime.UtcNow));

    private async Task<LoginResponse> MontarRespostaAsync(Usuario usuario, string refreshToken)
    {
        var perfis = await _usuarios.GetRolesAsync(usuario);
        var (accessToken, expiraEm) = _tokens.GerarAccessToken(usuario, perfis);
        return new LoginResponse(accessToken, expiraEm, refreshToken);
    }

    private RefreshToken NovoRegistro(string usuarioId, string refreshToken) => new()
    {
        UsuarioId = usuarioId,
        TokenHash = CalcularHash(refreshToken),
        CriadoEm = DateTime.UtcNow,
        ExpiraEm = DateTime.UtcNow.AddDays(_opcoes.DiasDoRefreshToken)
    };

    private static string GerarRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    private static string CalcularHash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
