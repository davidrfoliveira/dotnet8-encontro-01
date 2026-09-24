using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Seguradora.Api.Controllers;

[Authorize]
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<Usuario> _usuarios;
    private readonly SignInManager<Usuario> _login;
    private readonly IServicoDeSessao _sessao;

    public AuthController(UserManager<Usuario> usuarios, SignInManager<Usuario> login, IServicoDeSessao sessao)
    {
        _usuarios = usuarios;
        _login = login;
        _sessao = sessao;
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

        return Ok(await _sessao.IniciarAsync(usuario));
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponse>> Renovar(RefreshRequest requisicao)
    {
        var resposta = await _sessao.RenovarAsync(requisicao.RefreshToken);
        return resposta is null
            ? Unauthorized(new { erro = "Refresh token inválido ou expirado." })
            : Ok(resposta);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Sair(RefreshRequest requisicao)
    {
        await _sessao.RevogarAsync(requisicao.RefreshToken);
        return NoContent();
    }

    [HttpPost("sair-de-todos")]
    public async Task<IActionResult> SairDeTodos()
    {
        var usuario = await _usuarios.FindByIdAsync(User.FindFirst("sub")!.Value);
        if (usuario is null)
            return Unauthorized();

        await _sessao.RevogarTodasAsync(usuario);
        return NoContent();
    }
}
