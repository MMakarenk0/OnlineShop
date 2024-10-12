namespace OnlineShop.DataLayer.Entities;

public class Item : IEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<ItemImage> Images { get; set; } = new List<ItemImage>();
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public ICollection<ItemTrait> ItemTraits { get; set; }
}
