namespace OnlineShop.BLL.Dtos.Create;

public class CreateCategoryDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public ICollection<Guid>? TraitIds { get; set; }
}
