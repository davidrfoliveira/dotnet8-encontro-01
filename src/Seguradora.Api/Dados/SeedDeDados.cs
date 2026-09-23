using Microsoft.EntityFrameworkCore;
public static class SeedDeDados
{
    public static async Task PopularAsync(IServiceProvider servicos)
    {
        using var escopo = servicos.CreateScope();
        var contexto = escopo.ServiceProvider.GetRequiredService<SeguradoraDbContext>();
        if (await contexto.Apolices.AnyAsync())
            return;

        var maria = await contexto.Segurados.FirstAsync(s => s.Id == "SEG00001");
        var joao = await contexto.Segurados.FirstAsync(s => s.Id == "SEG00002");

        var auto = new ApoliceAutomovel
        {
            Id = "AUTO-2026-001",
            Segurado = joao,
            ValorSegurado = 85_000m,
            Inicio = new DateTime(2026, 1, 10),
            Placa = "ABC1D23",
            AnoFabricacao = 2012
        };

        auto.AdicionarCobertura(new Cobertura { Id = "COB-1", Tipo = TipoCobertura.Colisao, Descricao = "Colisão e capotagem", LimiteIndenizacao = 85_000m });
        auto.AdicionarCobertura(new Cobertura { Id = "COB-2", Tipo = TipoCobertura.Roubo, Descricao = "Roubo e furto", LimiteIndenizacao = 85_000m });
        auto.Ativar();

        var residencial = new ApoliceResidencial
        {
            Id = "RES-2026-002",
            Segurado = maria,
            ValorSegurado = 450_000m,
            Inicio = new DateTime(2026, 2, 1),
            AreaConstruida = 180m,
            PossuiAlarme = true
        };

        residencial.AdicionarCobertura(new Cobertura
        {
            Id = "COB-3",
            Tipo = TipoCobertura.Incendio,
            Descricao = "Incêndio e explosão",
            LimiteIndenizacao = 450_000m
        });

        residencial.Ativar();

        var vida = new ApoliceVida
        {
            Id = "VIDA-2026-003",
            Segurado = maria,
            ValorSegurado = 200_000m,
            Inicio = new DateTime(2026, 3, 1),
            Fumante = false
        };

        vida.AdicionarCobertura(new Cobertura { Id = "COB-4", Tipo = TipoCobertura.Morte, Descricao = "Morte por qualquer causa", LimiteIndenizacao = 200_000m });
        {
            var sinistro = new Sinistro
            {
                Id = "SIN-2026-001",
                Apolice = auto,
                TipoOcorrencia = TipoCobertura.Colisao,
                DataOcorrencia = new DateTime(2026, 5, 10),
                Descricao = "Colisão traseira em cruzamento",
                ValorPleiteado = 12_000m
            };
            
            contexto.Apolices.AddRange(auto, residencial, vida);
            contexto.Sinistros.Add(sinistro);
            await contexto.SaveChangesAsync();
        }


    }
}