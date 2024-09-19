namespace OnlineShop.DataLayer.Entities;

public class ItemImage : IEntity
{
    public string ImageUrl { get; set; }
    public string AltText { get; set; }
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
}
