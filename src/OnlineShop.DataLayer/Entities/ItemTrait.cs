namespace OnlineShop.DataLayer.Entities;

public class ItemTrait : IEntity
{
    public Guid ItemId { get; set; }
    public Item Item { get; set; }
    public Guid TraitId { get; set; }
    public Trait Trait { get; set; }
    public string Value { get; set; }
}
