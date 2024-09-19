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

        builder.HasMany(i => i.ItemCategories)
            .WithOne(ic => ic.Item)
            .HasForeignKey(ic => ic.ItemId);

        builder.HasMany(i => i.Images)
            .WithOne(i => i.Item)
            .HasForeignKey(ic => ic.ItemId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
