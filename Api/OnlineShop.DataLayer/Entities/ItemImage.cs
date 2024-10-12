namespace OnlineShop.DataLayer.Entities;

public class ItemImage : IEntity
{
    public string FileName { get; set; } // File name for generating SAS-links
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
}
