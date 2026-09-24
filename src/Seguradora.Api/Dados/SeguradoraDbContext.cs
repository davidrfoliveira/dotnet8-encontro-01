using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class SeguradoraDbContext : IdentityDbContext<Usuario>
{
    public SeguradoraDbContext(DbContextOptions<SeguradoraDbContext> opcoes)
        : base(opcoes)
    {

    }

    //tabela no banco de dados
    public DbSet<Segurado> Segurados => Set<Segurado>();
    public DbSet<Apolice> Apolices => Set<Apolice>();
    public DbSet<Sinistro> Sinistros => Set<Sinistro>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    //configura a tabela no banco
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    base.OnModelCreating(modelBuilder);
       modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeguradoraDbContext).Assembly);
    }


}