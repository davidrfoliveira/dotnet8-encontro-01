using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApoliceConfiguration : IEntityTypeConfiguration<Apolice>
{
    public void Configure(EntityTypeBuilder<Apolice> entidade)
    {
        entidade.ToTable("Apolices");
        entidade.HasKey(a => a.Id);
        entidade.Property(a => a.Id).HasMaxLength(20);
        entidade.Property(a => a.ValorSegurado).HasPrecision(18, 2);
        entidade.Property(a => a.Inicio).IsRequired();
        entidade.Property(a => a.VigenciaEmMeses).IsRequired();
        entidade.Property(a => a.Situacao)
        .HasConversion<string>()
        .HasMaxLength(20);

        entidade.HasOne(a => a.Segurado)
        .WithMany()
        .HasForeignKey("SeguradoId")
        .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);

        entidade.HasMany(a => a.Coberturas)
        .WithOne()
        .HasForeignKey("ApoliceId")
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

        entidade.Navigation(a => a.Coberturas)
        .HasField("_coberturas")
        .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        entidade.HasDiscriminator<string>("Tipo")
        .HasValue<ApoliceAutomovel>("Automovel")
        .HasValue<ApoliceResidencial>("Residencial")
        .HasValue<ApoliceVida>("Vida");
    }
}