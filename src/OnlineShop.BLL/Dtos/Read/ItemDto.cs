namespace OnlineShop.BLL.Dtos.Read
{
    public class ItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public ICollection<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public ICollection<string> ImagesUrls { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<TraitDto> Traits { get; set; }
    }
}
