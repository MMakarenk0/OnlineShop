namespace OnlineShop.DataLayer.Entities;

public class Trait : IEntity
{
    public string Name { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<ItemTrait> ItemTraits { get; set; }
}
