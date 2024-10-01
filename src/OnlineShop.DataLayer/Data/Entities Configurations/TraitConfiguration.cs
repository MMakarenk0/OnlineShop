using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Entities_Configurations;

public class TraitConfiguration : IEntityTypeConfiguration<Trait>
{
    public void Configure(EntityTypeBuilder<Trait> builder)
    {
        builder.Property(c => c.Id)
            .IsRequired();

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(c => c.Categories)
            .WithMany(i => i.Traits)
            .UsingEntity(ic => ic.ToTable("CategoryTraits"));

        builder.HasMany(t => t.ItemTraits)
               .WithOne(it => it.Trait)
               .HasForeignKey(it => it.TraitId);
    }
}
