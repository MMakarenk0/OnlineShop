namespace OnlineShop.BLL.Dtos.Read;

public class ItemFilterDto
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int? MinQuantityInStock { get; set; }
    public int? MaxQuantityInStock { get; set; }
    public Guid? CategoryId { get; set; }
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public ICollection<Guid>? TraitIds { get; set; }
}
