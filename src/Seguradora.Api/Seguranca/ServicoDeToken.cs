using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
public interface IServicoDeToken
{
    (string Token, DateTime ExpiraEm) GerarAccessToken(Usuario usuario, IEnumerable<string> perfis);
}
public class ServicoDeToken : IServicoDeToken
{

    private readonly OpcoesJwt _opcoes;
    public ServicoDeToken(IOptions<OpcoesJwt> opcoes)
    {
        _opcoes = opcoes.Value;
    }
    public (string Token, DateTime ExpiraEm) GerarAccessToken(Usuario usuario, IEnumerable<string> perfis)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id),
            new(JwtRegisteredClaimNames.Email, usuario.Email!),
            new(JwtRegisteredClaimNames.Name, usuario.NomeCompleto),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(perfis.Select(perfil => new Claim("role", perfil)));
        if (usuario.SeguradoId is not null)
            claims.Add(new Claim("segurado_id", usuario.SeguradoId));
        if (usuario.Alcada is not null)
            claims.Add(new Claim("alcada", usuario.Alcada.Value.ToString(CultureInfo.InvariantCulture)));

        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opcoes.Chave));
        var credenciais = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);
        var expiraEm = DateTime.UtcNow.AddMinutes(_opcoes.MinutosDeExpiracao);

        var token = new JwtSecurityToken(
        issuer: _opcoes.Emissor,
        audience: _opcoes.Audiencia,
        claims: claims,
        notBefore: DateTime.UtcNow,
        expires: expiraEm,
        signingCredentials: credenciais);
        
        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}