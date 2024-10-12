namespace OnlineShop.BLL.Dtos.Update;

public class UpdateCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public Guid? ParentId { get; set; }
    public ICollection<Guid>? TraitIds { get; set; }
}