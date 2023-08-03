using GenersOfMoviesAPIS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace GenersOfMoviesAPIS.ConfigurationEntities
{
    public class genereConfiguration : IEntityTypeConfiguration<Genere>
    {
        public void Configure(EntityTypeBuilder<Genere> builder)
        {
            builder.ToTable("Generes").HasKey(e => e.Id);

            builder.Property(e => e.Name).HasColumnType("varchar").HasMaxLength(250).IsRequired();

            
           
        }
    }
}
