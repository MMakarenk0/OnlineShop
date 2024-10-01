using Microsoft.AspNetCore.Http;
using OnlineShop.BLL.Dtos.Read;

namespace OnlineShop.BLL.Dtos.Create;

public class CreateItemDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int QuantityInStock { get; set; }
    public ICollection<Guid>? CategoryIds { get; set; }
    public ICollection<IFormFile>? ImageFiles { get; set; }
    public ICollection<TraitValueDto>? TraitValues { get; set; }
}
