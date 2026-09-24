using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entidade)
    {
        entidade.ToTable("RefreshTokens");
        entidade.HasKey(t => t.Id);

        entidade.Property(t => t.TokenHash)
            .IsRequired()
            .HasMaxLength(64);

        entidade.HasIndex(t => t.TokenHash).IsUnique();

        entidade.HasOne<Usuario>()
            .WithMany()
            .HasForeignKey(t => t.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
