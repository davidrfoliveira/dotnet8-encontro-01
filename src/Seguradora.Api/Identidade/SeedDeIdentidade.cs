using Microsoft.AspNetCore.Identity;
public static class SeedDeIdentidade
{
    private static readonly string[] Perfis = { "Admin", "Corretor", "Regulador", "Segurado" };
    public static async Task PopularAsync(IServiceProvider servicos)
    {
        using var escopo = servicos.CreateScope();
        var perfis = escopo.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var usuarios = escopo.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
        var configuracao = escopo.ServiceProvider.GetRequiredService<IConfiguration>();
        var senha = configuracao["Seed:SenhaPadrao"]
        ?? throw new InvalidOperationException("Configure Seed:SenhaPadrao no appsettings.Development.json.");

        foreach (var perfil in Perfis)
        {
            if (!await perfis.RoleExistsAsync(perfil))
                await perfis.CreateAsync(new IdentityRole(perfil));

        }

        await CriarAsync(usuarios, senha, "Admin", "admin@seguradora.com", "Ana Administradora", alcada: 1_000_000m);
        await CriarAsync(usuarios, senha, "Corretor", "corretor@seguradora.com", "Carlos Corretor");
        await CriarAsync(usuarios, senha, "Regulador", "regulador@seguradora.com", "Rita Reguladora", alcada: 20_000m);
        await CriarAsync(usuarios, senha, "Segurado", "maria@seguradora.com", "Maria Silva", seguradoId: "SEG00001");
        await CriarAsync(usuarios, senha, "Segurado", "joao@seguradora.com", "João Souza", seguradoId: "SEG00002");
    }
    private static async Task CriarAsync(
        UserManager<Usuario> usuarios,
        string senha,
        string perfil,
        string email,
        string nome,
        decimal? alcada = null,
        string? seguradoId = null)
    {
        if (await usuarios.FindByEmailAsync(email) is not null)
            return;

        var usuario = new Usuario
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NomeCompleto = nome,
            Alcada = alcada,
            SeguradoId = seguradoId
        };

        var resultado = await usuarios.CreateAsync(usuario, senha);
        if (!resultado.Succeeded)
            throw new InvalidOperationException(string.Join("; ", resultado.Errors.Select(e => e.Description)));
        await usuarios.AddToRoleAsync(usuario, perfil);
    }
}