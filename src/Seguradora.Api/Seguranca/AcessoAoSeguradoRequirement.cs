using Microsoft.AspNetCore.Authorization;

public class AcessoAoSeguradoRequirement : IAuthorizationRequirement
{
}

public class AcessoAoSeguradoHandler : AuthorizationHandler<AcessoAoSeguradoRequirement, Apolice>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext contexto,
        AcessoAoSeguradoRequirement requisito,
        Apolice apolice)
    {
        var funcionario = contexto.User.IsInRole("Admin")
            || contexto.User.IsInRole("Corretor")
            || contexto.User.IsInRole("Regulador");

        var donoDaApolice = contexto.User.FindFirst("segurado_id")?.Value == apolice.Segurado.Id;

        if (funcionario || donoDaApolice)
            contexto.Succeed(requisito);

        return Task.CompletedTask;
    }
}
