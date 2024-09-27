using Microsoft.AspNetCore.Http;

namespace OnlineShop.BLL.Dtos.Update;

public class UpdateItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public ICollection<Guid> CategoryIds { get; set; }
    public ICollection<IFormFile>? ImageFiles { get; set; }
}
