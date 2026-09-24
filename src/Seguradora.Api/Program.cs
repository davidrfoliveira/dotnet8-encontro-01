// using Microsoft.AspNetCore.Http.HttpResults;
using Seguradora.Api.Diagnostico;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SeguradoraDbContext>(opcoes =>
opcoes.UseSqlite(builder.Configuration.GetConnectionString("Seguradora")));



// Add services to the container.
// builder.Services.AddSingleton<ISeguradoRepositorio, SeguradoRepositorioEmMemoria>();

builder.Services.AddScoped<ISeguradoRepositorio, SeguradoEntityFrameworkRepositorio>();
builder.Services.AddScoped<IApoliceRepositorio, ApoliceEntityFrameworkRepositorio>();
builder.Services.AddScoped<ISinistroRepositorio, SinistroEntityFrameworkRepositorio>();


builder.Services.AddTransient<IOperacaoTransient, OperacaoDiagnostico>();
builder.Services.AddScoped<IOperacaoScoped, OperacaoDiagnostico>();
builder.Services.AddSingleton<IOperacaoSingleton, OperacaoDiagnostico>();
builder.Services.AddScoped<IServicoDiagnostico, ServicoDiagnostico>();
// builder.Services.AddScoped<IApoliceRepositorio, ApoliceRepositorioEmMemoria>();

builder.Services.AddOptions<OpcoesDaSeguradora>()
    .Bind(builder.Configuration.GetSection("Seguradora"))
    .Validate(o => o.FranquiaMinima > 0, "FranquiaMinima deve ser maior que zero.")
    .ValidateOnStart();




//identity
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddIdentityCore<Usuario>(opcoes =>
{
    opcoes.Password.RequiredLength = 8;
    opcoes.User.RequireUniqueEmail = true;
    opcoes.Lockout.MaxFailedAccessAttempts = 5;
    opcoes.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddRoles<IdentityRole>().AddEntityFrameworkStores<SeguradoraDbContext>().AddSignInManager();

builder.Services.AddOptions<OpcoesJwt>()
.Bind(builder.Configuration.GetSection(OpcoesJwt.Secao))
.Validate(o => o.Chave.Length >= 32, "Jwt:Chave precisa ter pelo menos 32 caracteres.")
.ValidateOnStart();
builder.Services.AddScoped<IServicoDeToken, ServicoDeToken>();
builder.Services.AddScoped<IServicoDeSessao, ServicoDeSessao>();

var jwt = builder.Configuration.GetSection(OpcoesJwt.Secao).Get<OpcoesJwt>() ?? new OpcoesJwt();
builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opcoes =>
{
    opcoes.MapInboundClaims = false;
    opcoes.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwt.Emissor,
        ValidateAudience = true,
        ValidAudience = jwt.Audiencia,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Chave)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "name",
        RoleClaimType = "role"
    };

    opcoes.Events = new JwtBearerEvents
    {
        OnTokenValidated = async contexto =>
        {
            var usuarios = contexto.HttpContext.RequestServices.GetRequiredService<UserManager<Usuario>>();
            var id = contexto.Principal?.FindFirst("sub")?.Value;
            var stamp = contexto.Principal?.FindFirst("stamp")?.Value;

            var usuario = id is null ? null : await usuarios.FindByIdAsync(id);
            if (usuario is null || usuario.SecurityStamp != stamp)
                contexto.Fail("Token revogado.");
        }
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AnalistaDeSinistros", politica => politica
        .RequireRole("Regulador", "Admin")
        .RequireClaim("alcada"))
    .AddPolicy("AlcadaSuficiente", politica => politica.AddRequirements(new AlcadaRequirement()))
    .AddPolicy("AcessoAoSegurado", politica => politica.AddRequirements(new AcessoAoSeguradoRequirement()))
    .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddSingleton<IAuthorizationHandler, AlcadaHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, AcessoAoSeguradoHandler>();


//
// builder.Services.AddControllers();

builder.Services.AddControllers()
.AddJsonOptions(opcoes => opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(opcoes =>
{
    opcoes.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole apenas o token JWT (sem a palavra Bearer)."
    });

    opcoes.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});





var app = builder.Build();

app.UseMiddleware<TratamentoDeErroMiddleware>();

app.Use(async (contexto, proximo) =>
{
    contexto.Response.Headers["X-Content-Type-Options"] = "nosniff";
    contexto.Response.Headers["X-Frame-Options"] = "DENY";
    contexto.Response.Headers["Referrer-Policy"] = "no-referrer";
    contexto.Response.Headers["Cache-Control"] = "no-store";
    await proximo();
});

app.MapGet("/diagnostico/forcar-erro/{tipo}", (string tipo) =>
{
    if (tipo == "argument") throw new ArgumentException("CPF inválido de propósito");
    if (tipo == "operacao") throw new InvalidOperationException("Regra de negócio violada de propósito");
    throw new Exception("Erro genérico de propósito");
});

app.MapGet("/diagnostico/ciclo-de-vida", (IOperacaoTransient t1, IOperacaoTransient t2,
IOperacaoScoped s1, IOperacaoScoped s2,
IOperacaoSingleton g1, IOperacaoSingleton g2,
IServicoDiagnostico servico) =>
Results.Ok(new
{
    transient = new { direto1 = t1.OperacaoId, direto2 = t2.OperacaoId, viaServico = servico.TransientId },
    scoped = new { direto1 = s1.OperacaoId, direto2 = s2.OperacaoId, viaServico = servico.ScopedId },
    singleton = new { direto1 = g1.OperacaoId, direto2 = g2.OperacaoId, viaServico = servico.SingletonId }
}));




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await SeedDeDados.PopularAsync(app.Services);
    await SeedDeIdentidade.PopularAsync(app.Services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();





app.Run();
