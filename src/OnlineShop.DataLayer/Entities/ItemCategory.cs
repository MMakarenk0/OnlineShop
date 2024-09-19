namespace OnlineShop.DataLayer.Entities;

public class ItemCategory : IEntity
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

}
