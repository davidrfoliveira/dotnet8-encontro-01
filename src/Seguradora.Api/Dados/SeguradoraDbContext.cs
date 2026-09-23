using Microsoft.EntityFrameworkCore;

public class SeguradoraDbContext : DbContext
{
    public SeguradoraDbContext(DbContextOptions<SeguradoraDbContext> opcoes)
        : base(opcoes)
    {

    }

    //tabela no banco de dados
    public DbSet<Segurado> Segurados => Set<Segurado>();
    public DbSet<Apolice> Apolices => Set<Apolice>();
    public DbSet<Sinistro> Sinistros => Set<Sinistro>();

    //configura a tabela no banco
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
       modelBuilder.ApplyConfigurationsFromAssembly(typeof(SeguradoraDbContext).Assembly);
    }


}