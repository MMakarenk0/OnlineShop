using DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer;

public class DataSeeder
{
    private readonly IUnitOfWork _uof;

    public DataSeeder(IUnitOfWork uof)
    {
        _uof = uof;
    }

    public async Task Seed()
    {
        if (!_uof.CategoryRepository.GetAll().Any())
        {
            await SeedCategories();
        }

        if (!_uof.TraitRepository.GetAll().Any())
        {
            await SeedTraits();
        }

        if (!_uof.ItemRepository.GetAll().Any())
        {
            await SeedItems();
        }

        await _uof.SaveChangesAsync();
    }

    private async Task SeedCategories()
    {
        var categories = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Laptops" },
            new Category { Id = Guid.NewGuid(), Name = "Smartphones" },
            new Category { Id = Guid.NewGuid(), Name = "Tablets" }
        };

        foreach (var category in categories)
        {
            await _uof.CategoryRepository.Create(category);
        }
    }

    private async Task SeedTraits()
    {
        var traits = new List<Trait>
        {
            new Trait { Id = Guid.NewGuid(), Name = "Processor" },
            new Trait { Id = Guid.NewGuid(), Name = "RAM" },
            new Trait { Id = Guid.NewGuid(), Name = "Storage" },
            new Trait { Id = Guid.NewGuid(), Name = "Camera" }
        };

        foreach (var trait in traits)
        {
            await _uof.TraitRepository.Create(trait);
        }
    }

    private async Task SeedItems()
    {
        var laptopsCategory = _uof.CategoryRepository.GetAll().First(c => c.Name == "Laptops");
        var traits = _uof.TraitRepository.GetAll();

        var items = new List<Item>
        {
            new Item
            {
                Id = Guid.NewGuid(),
                Name = "Acer Nitro 5",
                Description = "Gaming laptop",
                Price = 1000m,
                QuantityInStock = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ItemTraits = new List<ItemTrait>
                {
                    new ItemTrait { Id = Guid.NewGuid(), TraitId = traits.First(t => t.Name == "Processor").Id, Value = "Intel Core i7" },
                    new ItemTrait { Id = Guid.NewGuid(), TraitId = traits.First(t => t.Name == "RAM").Id, Value = "16GB" },
                    new ItemTrait { Id = Guid.NewGuid(), TraitId = traits.First(t => t.Name == "Storage").Id, Value = "512GB SSD" }
                }
            }
        };
        foreach (var item in items)
        {
            await _uof.ItemRepository.Create(item);
        }
    }
}
