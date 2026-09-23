using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApoliceAutomovelConfiguration : IEntityTypeConfiguration<ApoliceAutomovel>
{
    public void Configure(EntityTypeBuilder<ApoliceAutomovel> entidade)
    {
        entidade.Property(a => a.Placa).HasMaxLength(8);
    }
}