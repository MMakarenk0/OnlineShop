using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Infrastructure;

public class ItemCategoryConfiguration : IEntityTypeConfiguration<ItemCategory>
{
    public void Configure(EntityTypeBuilder<ItemCategory> builder)
    {
        builder.HasKey(ic => new { ic.ItemId, ic.CategoryId });

        builder.HasOne(ic => ic.Item)
               .WithMany(i => i.ItemCategories)
               .HasForeignKey(ic => ic.ItemId);

        builder.HasOne(ic => ic.Category)
               .WithMany(c => c.ItemCategories)
               .HasForeignKey(ic => ic.CategoryId);
    }
}