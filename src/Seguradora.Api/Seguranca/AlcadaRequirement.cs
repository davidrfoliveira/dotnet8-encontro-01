using System.Globalization;
using Microsoft.AspNetCore.Authorization;

public class AlcadaRequirement : IAuthorizationRequirement
{
}

public class AlcadaHandler : AuthorizationHandler<AlcadaRequirement, Sinistro>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext contexto,
        AlcadaRequirement requisito,
        Sinistro sinistro)
    {
        var claim = contexto.User.FindFirst("alcada")?.Value;

        if (decimal.TryParse(claim, NumberStyles.Number, CultureInfo.InvariantCulture, out var alcada)
            && alcada >= sinistro.ValorPleiteado)
        {
            contexto.Succeed(requisito);
        }

        return Task.CompletedTask;
    }
}
