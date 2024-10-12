using Microsoft.EntityFrameworkCore;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Data.Repositories.Realization;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.UnitTests.DataLayerTests
{
    public class ItemTraitRepositoryTests
    {
        private readonly ItemTraitRepository _itemTraitRepository;
        private readonly OnlineShopDbContext _dbContext;

        public ItemTraitRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<OnlineShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new OnlineShopDbContext(options);
            _itemTraitRepository = new ItemTraitRepository(_dbContext);
        }

        [Fact]
        public async Task Create_ShouldAddItemTrait()
        {
            // Arrange
            var itemTrait = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value"
            };

            // Act
            var result = await _itemTraitRepository.Create(itemTrait);
            await _dbContext.SaveChangesAsync();

            // Assert
            var createdItemTrait = await _dbContext.ItemTraits
                .FindAsync(itemTrait.ItemId, itemTrait.TraitId);
            Assert.NotNull(createdItemTrait);
            Assert.Equal(itemTrait.Value, createdItemTrait.Value);
        }

        [Fact]
        public async Task Delete_ShouldRemoveItemTrait()
        {
            // Arrange
            var itemTrait = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value"
            };

            await _dbContext.ItemTraits.AddAsync(itemTrait);
            await _dbContext.SaveChangesAsync();

            // Act
            await _itemTraitRepository.Delete(itemTrait.ItemId, itemTrait.TraitId);
            await _dbContext.SaveChangesAsync();

            // Assert
            var deletedItemTrait = await _dbContext.ItemTraits
                .FindAsync(itemTrait.ItemId, itemTrait.TraitId);
            Assert.Null(deletedItemTrait);
        }

        [Fact]
        public async Task Find_ShouldReturnItemTrait()
        {
            // Arrange
            var itemTrait = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value"
            };

            await _dbContext.ItemTraits.AddAsync(itemTrait);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _itemTraitRepository.Find(itemTrait.ItemId, itemTrait.TraitId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(itemTrait.Value, result.Value);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllItemTraits()
        {
            // Arrange
            var itemTrait1 = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value 1"
            };
            var itemTrait2 = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value 2"
            };

            await _dbContext.ItemTraits.AddRangeAsync(itemTrait1, itemTrait2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = _itemTraitRepository.GetAll();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredItemTraits()
        {
            // Arrange
            var itemTrait1 = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value 1"
            };
            var itemTrait2 = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value 2"
            };

            await _dbContext.ItemTraits.AddRangeAsync(itemTrait1, itemTrait2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _itemTraitRepository.GetAllAsync(x => x.Value.Contains("2"));

            // Assert
            Assert.Single(result);
            Assert.Equal(itemTrait2.Value, result.First().Value);
        }

        [Fact]
        public async Task Update_ShouldModifyItemTrait()
        {
            // Arrange
            var itemTrait = new ItemTrait
            {
                ItemId = Guid.NewGuid(),
                TraitId = Guid.NewGuid(),
                Value = "Trait Value"
            };

            await _dbContext.ItemTraits.AddAsync(itemTrait);
            await _dbContext.SaveChangesAsync();

            // Act
            itemTrait.Value = "Updated Trait Value";
            var result = await _itemTraitRepository.Update(itemTrait);
            await _dbContext.SaveChangesAsync();

            // Assert
            var updatedItemTrait = await _dbContext.ItemTraits
                .FindAsync(itemTrait.ItemId, itemTrait.TraitId);
            Assert.NotNull(updatedItemTrait);
            Assert.Equal("Updated Trait Value", updatedItemTrait.Value);
        }
    }
}
