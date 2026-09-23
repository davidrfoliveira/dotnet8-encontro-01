using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SeguradoConfiguration : IEntityTypeConfiguration<Segurado>
{
    public void Configure(EntityTypeBuilder<Segurado> entidade)
    {
         entidade.ToTable("Segurados");
            entidade.HasKey(s => s.Id);
            entidade.Property(s => s.Id).HasMaxLength(8);
            entidade.Property(s => s.Nome)
            .IsRequired()
            .HasMaxLength(120);
            entidade.Property(s => s.Documento)
            .HasColumnName("Cpf")
            .HasConversion(cpf => cpf.Numero, numero => new Cpf(numero))
            .IsRequired()
            .HasMaxLength(11);
            entidade.Property(s => s.DataNascimento).IsRequired();
            entidade.Property(s => s.DataCadastro).IsRequired();

            entidade.HasData(new Segurado
            {
                Id = "SEG00001",
                Nome = "Maria Silva",
                Documento = new Cpf("529.982.247-25"),
                DataNascimento = new DateTime(1988, 4, 12),
                DataCadastro = new DateTime(2026, 1, 1)
            },
            new Segurado
            {
                Id = "SEG00002",
                Nome = "João Souza",
                Documento = new Cpf("11144477735"),
                DataNascimento = new DateTime(2004, 9, 30),
                DataCadastro = new DateTime(2026, 1, 1)
            }
            );
    }
}