namespace OnlineShop.BLL.Dtos.Read;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
    public ICollection<CategoryDto> SubCategories { get; set; }
}
