using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Seguradora.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _usuarios;
    private readonly SignInManager<Usuario> _login;
    private readonly IServicoDeToken _tokens;
    public AuthController(UserManager<Usuario> usuarios, SignInManager<Usuario> login, IServicoDeToken tokens)
    {
        _usuarios = usuarios;
        _login = login;
        _tokens = tokens;
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest requisicao)
    {
        var usuario = await _usuarios.FindByEmailAsync(requisicao.Email);
        if (usuario is null)
            return Unauthorized(new { erro = "Credenciais inválidas." });
        var resultado = await _login.CheckPasswordSignInAsync(usuario, requisicao.Senha, lockoutOnFailure: true);
        if (!resultado.Succeeded)
            return Unauthorized(new { erro = "Credenciais inválidas." });
        var perfis = await _usuarios.GetRolesAsync(usuario);
        var (token, expiraEm) = _tokens.GerarAccessToken(usuario, perfis);
        return Ok(new LoginResponse(token, expiraEm));
    }
}