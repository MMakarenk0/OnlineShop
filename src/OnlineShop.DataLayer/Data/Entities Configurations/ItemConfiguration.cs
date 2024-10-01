using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Entities_Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.Property(i => i.Id)
            .IsRequired();

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Description)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Price)
            .IsRequired();

        builder.Property(i => i.QuantityInStock)
            .IsRequired();

        builder.HasMany(i => i.Categories)
            .WithMany(c => c.Items)
            .UsingEntity(ic => ic.ToTable("ItemCategories"));
            
        builder.HasMany(i => i.Images)
            .WithOne(i => i.Item)
            .HasForeignKey(ic => ic.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(i => i.ItemTraits)
               .WithOne(it => it.Item)
               .HasForeignKey(it => it.ItemId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
