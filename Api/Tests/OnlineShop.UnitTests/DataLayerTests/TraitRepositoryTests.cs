using Microsoft.EntityFrameworkCore;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Data.Repositories.Realization;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.UnitTests.DataLayerTests
{
    public class TraitRepositoryTests
    {
        private readonly TraitRepository _traitRepository;
        private readonly OnlineShopDbContext _dbContext;

        public TraitRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<OnlineShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new OnlineShopDbContext(options);
            _traitRepository = new TraitRepository(_dbContext);
        }

        [Fact]
        public async Task Create_ShouldAddTrait()
        {
            // Arrange
            var trait = new Trait
            {
                Name = "Test Trait"
            };

            // Act
            var result = await _traitRepository.Create(trait);
            await _dbContext.SaveChangesAsync();

            // Assert
            var createdTrait = await _dbContext.Traits.FindAsync(result.Id);
            Assert.NotNull(createdTrait);
            Assert.Equal(trait.Name, createdTrait.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveTrait()
        {
            // Arrange
            var trait = new Trait
            {
                Id = Guid.NewGuid(),
                Name = "Test Trait"
            };

            await _dbContext.Traits.AddAsync(trait);
            await _dbContext.SaveChangesAsync();

            // Act
            await _traitRepository.Delete(trait.Id);
            await _dbContext.SaveChangesAsync();

            // Assert
            var deletedTrait = await _dbContext.Traits.FindAsync(trait.Id);
            Assert.Null(deletedTrait);
        }

        [Fact]
        public async Task Find_ShouldReturnTrait()
        {
            // Arrange
            var trait = new Trait
            {
                Id = Guid.NewGuid(),
                Name = "Test Trait"
            };

            await _dbContext.Traits.AddAsync(trait);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _traitRepository.Find(trait.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trait.Name, result.Name);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllTraits()
        {
            // Arrange
            var trait1 = new Trait
            {
                Id = Guid.NewGuid(),
                Name = "Trait 1"
            };
            var trait2 = new Trait
            {
                Id = Guid.NewGuid(),
                Name = "Trait 2"
            };

            await _dbContext.Traits.AddRangeAsync(trait1, trait2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = _traitRepository.GetAll();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Update_ShouldModifyTrait()
        {
            // Arrange
            var trait = new Trait
            {
                Id = Guid.NewGuid(),
                Name = "Test Trait"
            };

            await _dbContext.Traits.AddAsync(trait);
            await _dbContext.SaveChangesAsync();

            // Act
            trait.Name = "Updated Trait";
            var result = await _traitRepository.Update(trait);
            await _dbContext.SaveChangesAsync();

            // Assert
            var updatedTrait = await _dbContext.Traits.FindAsync(trait.Id);
            Assert.NotNull(updatedTrait);
            Assert.Equal("Updated Trait", updatedTrait.Name);
        }
    }
}
