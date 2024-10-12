using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Entities_Configurations;

public class ItemTraitConfiguration : IEntityTypeConfiguration<ItemTrait>
{
    public void Configure(EntityTypeBuilder<ItemTrait> builder)
    {
        builder.HasKey(it => new { it.ItemId, it.TraitId });

        builder.Property(it => it.Value)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(it => it.Item)
            .WithMany(i => i.ItemTraits)
            .HasForeignKey(it => it.ItemId);

        builder.HasOne(it => it.Trait)
            .WithMany(t => t.ItemTraits)
            .HasForeignKey(it => it.TraitId);
    }
}
