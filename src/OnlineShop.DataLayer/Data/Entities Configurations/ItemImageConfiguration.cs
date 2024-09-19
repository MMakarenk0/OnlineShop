using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Entities_Configurations;

public class ItemImageConfiguration : IEntityTypeConfiguration<ItemImage>
{
    public void Configure(EntityTypeBuilder<ItemImage> builder)
    {
        builder.Property(ii => ii.Id).IsRequired();

        builder.Property(ii => ii.ImageUrl)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ii => ii.AltText)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(ii => ii.Item)
            .WithMany(i => i.Images)
            .HasForeignKey(i => i.ItemId);
    }
}
