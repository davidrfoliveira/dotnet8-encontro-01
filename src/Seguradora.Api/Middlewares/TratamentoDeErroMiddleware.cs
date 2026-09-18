public class TratamentoDeErroMiddleware
{
    private readonly RequestDelegate _proximo;
    private readonly ILogger<TratamentoDeErroMiddleware> _logger;

    public TratamentoDeErroMiddleware(RequestDelegate proximo, ILogger<TratamentoDeErroMiddleware> logger)
    {
        _proximo = proximo;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _proximo(contexto);
        }
        catch (ArgumentException ex)
        {
            contexto.Response.StatusCode = 400;
            await contexto.Response.WriteAsJsonAsync(new { erro = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            contexto.Response.StatusCode = 422;
            await contexto.Response.WriteAsJsonAsync(new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            contexto.Response.StatusCode = 500;
            _logger.LogError(ex, "Erro não tratado");
        }
    }
}
