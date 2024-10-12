using DataLayer.Data.Repositories.Realization;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.UnitTests.DataLayerTests;

public class ItemRepositoryTests
{
    private readonly ItemRepository _itemRepository;
    private readonly OnlineShopDbContext _dbContext;

    public ItemRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<OnlineShopDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new OnlineShopDbContext(options);

        _itemRepository = new ItemRepository(_dbContext);
    }
    [Fact]
    public async Task Create_ShouldAddItem()
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 100,
            QuantityInStock = 10
        };

        // Act
        var result = await _itemRepository.Create(item);
        await _dbContext.SaveChangesAsync();

        // Assert
        var createdItem = await _dbContext.Items.FindAsync(result.Id);
        Assert.NotNull(createdItem);
        Assert.Equal(item.Name, createdItem.Name);
    }

    [Fact]
    public async Task Delete_ShouldRemoveItem()
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 100,
            QuantityInStock = 10
        };

        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        // Act
        await _itemRepository.Delete(item.Id);
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedItem = await _dbContext.Items.FindAsync(item.Id);
        Assert.Null(deletedItem);
    }

    [Fact]
    public async Task Find_ShouldReturnItem()
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 100,
            QuantityInStock = 10
        };

        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _itemRepository.Find(item.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(item.Name, result.Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllItems()
    {
        // Arrange
        var item1 = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item 1",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 100,
            QuantityInStock = 10
        };
        var item2 = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item 2",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 200,
            QuantityInStock = 20
        };

        await _dbContext.Items.AddRangeAsync(item1, item2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = _itemRepository.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnFilteredItems()
    {
        // Arrange
        var item1 = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item 1",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 100,
            QuantityInStock = 10
        };
        var item2 = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item 2",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 200,
            QuantityInStock = 20
        };

        await _dbContext.Items.AddRangeAsync(item1, item2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _itemRepository.GetAllAsync(x => x.Price >= 150);

        // Assert
        Assert.Single(result);
        Assert.Equal(item2.Name, result.First().Name);
    }

    [Fact]
    public async Task Update_ShouldModifyItem()
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Name = "Test Item",
            Description = "Description",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Price = 100,
            QuantityInStock = 10
        };

        await _dbContext.Items.AddAsync(item);
        await _dbContext.SaveChangesAsync();

        // Act
        item.Name = "Updated Item";
        var result = await _itemRepository.Update(item);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedItem = await _dbContext.Items.FindAsync(item.Id);
        Assert.NotNull(updatedItem);
        Assert.Equal("Updated Item", updatedItem.Name);
    }
}
