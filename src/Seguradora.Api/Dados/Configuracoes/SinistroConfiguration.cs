using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SinistroConfiguration : IEntityTypeConfiguration<Sinistro>
{
    public void Configure(EntityTypeBuilder<Sinistro> entidade)
    {
        entidade.ToTable("Sinistros");
        entidade.HasKey(s => s.Id);
        entidade.Property(s => s.Id).HasMaxLength(20);
        entidade.Property(s => s.TipoOcorrencia)
        .HasConversion<string>()
        .HasMaxLength(30);
        entidade.Property(s => s.Descricao)
        .IsRequired()
        .HasMaxLength(500);
        entidade.Property(s => s.ValorPleiteado).HasPrecision(18, 2);
        entidade.Property(s => s.Situacao)
        .HasConversion<string>()
        .HasMaxLength(20);
        entidade.Property(s => s.MotivoNegativa).HasMaxLength(300);

        entidade.HasOne(s => s.Apolice)
        .WithMany()
        .HasForeignKey("ApoliceId")
        .IsRequired()
        .OnDelete(DeleteBehavior.Restrict);
    }
}