using Microsoft.AspNetCore.Http.HttpResults;
using Seguradora.Api.Diagnostico;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SeguradoraDbContext>(opcoes =>
opcoes.UseSqlite(builder.Configuration.GetConnectionString("Seguradora")));



// Add services to the container.
builder.Services.AddSingleton<ISeguradoRepositorio, SeguradoRepositorioEmMemoria>();
builder.Services.AddTransient<IOperacaoTransient, OperacaoDiagnostico>();
builder.Services.AddScoped<IOperacaoScoped, OperacaoDiagnostico>();
builder.Services.AddSingleton<IOperacaoSingleton, OperacaoDiagnostico>();
builder.Services.AddScoped<IServicoDiagnostico, ServicoDiagnostico>();
builder.Services.AddScoped<IApoliceRepositorio, ApoliceRepositorioEmMemoria>();

builder.Services.AddOptions<OpcoesDaSeguradora>()
    .Bind(builder.Configuration.GetSection("Seguradora"))
    .Validate(o => o.FranquiaMinima > 0, "FranquiaMinima deve ser maior que zero.")
    .ValidateOnStart();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseMiddleware<TratamentoDeErroMiddleware>();

app.MapGet("/diagnostico/forcar-erro/{tipo}", (string tipo) =>
{
    if (tipo == "argument") throw new ArgumentException("CPF inválido de propósito");
    if (tipo == "operacao") throw new InvalidOperationException("Regra de negócio violada de propósito");
    throw new Exception("Erro genérico de propósito");
});

app.MapGet("/diagnostico/ciclo-de-vida",  (IOperacaoTransient t1, IOperacaoTransient t2,
   IOperacaoScoped s1, IOperacaoScoped s2,
   IOperacaoSingleton g1, IOperacaoSingleton g2,
   IServicoDiagnostico servico) =>
  Results.Ok(new {    transient = new { direto1 = t1.OperacaoId, direto2 = t2.OperacaoId, viaServico = servico.TransientId },
    scoped = new { direto1 = s1.OperacaoId, direto2 = s2.OperacaoId, viaServico = servico.ScopedId },
    singleton = new { direto1 = g1.OperacaoId, direto2 = g2.OperacaoId, viaServico = servico.SingletonId }
  }));



var grupo = app.MapGroup("/apolices").WithTags("Apólices");

grupo.MapGet("/{id}",
    Results<Ok<Apolice>, NotFound> (string id, IApoliceRepositorio repo) =>
    {
        var a = repo.ObterPorId(id);
        return a is null ? TypedResults.NotFound() : TypedResults.Ok(a);
    });


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();





app.Run();
