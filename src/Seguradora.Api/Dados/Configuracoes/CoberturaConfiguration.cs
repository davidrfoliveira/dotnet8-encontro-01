using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CoberturaConfiguration : IEntityTypeConfiguration<Cobertura>
{
    public void Configure(EntityTypeBuilder<Cobertura> entidade)
    {
        entidade.ToTable("Coberturas");
        entidade.HasKey(c => c.Id);
        entidade.Property(c => c.Id).HasMaxLength(20);
        entidade.Property(c => c.Tipo)
        .HasConversion<string>()
        .HasMaxLength(30);

        entidade.Property(c => c.Descricao)
        .IsRequired()
        .HasMaxLength(200);
        entidade.Property(c => c.LimiteIndenizacao).HasPrecision(18, 2);
    }
}