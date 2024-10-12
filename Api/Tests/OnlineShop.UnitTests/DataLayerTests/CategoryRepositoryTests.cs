using DataLayer.Data.Repositories.Realization;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.UnitTests.DataLayerTests
{
    public class CategoryRepositoryTests
    {
        private readonly CategoryRepository _categoryRepository;
        private readonly OnlineShopDbContext _dbContext;

        public CategoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<OnlineShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new OnlineShopDbContext(options);
            _categoryRepository = new CategoryRepository(_dbContext);
        }

        [Fact]
        public async Task Create_ShouldAddCategory()
        {
            // Arrange
            var category = new Category
            {
                Name = "Test Category",
                ParentId = null
            };

            // Act
            var result = await _categoryRepository.Create(category);
            await _dbContext.SaveChangesAsync();

            // Assert
            var createdCategory = await _dbContext.Categories.FindAsync(result.Id);
            Assert.NotNull(createdCategory);
            Assert.Equal(category.Name, createdCategory.Name);
        }

        [Fact]
        public async Task Delete_ShouldRemoveCategory()
        {
            // Arrange
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                ParentId = null
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            await _categoryRepository.Delete(category.Id);
            await _dbContext.SaveChangesAsync();

            // Assert
            var deletedCategory = await _dbContext.Categories.FindAsync(category.Id);
            Assert.Null(deletedCategory);
        }

        [Fact]
        public async Task Find_ShouldReturnCategory()
        {
            // Arrange
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                ParentId = null
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.Find(category.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.Name, result.Name);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCategories()
        {
            // Arrange
            var category1 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category 1",
                ParentId = null
            };
            var category2 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category 2",
                ParentId = null
            };

            await _dbContext.Categories.AddRangeAsync(category1, category2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = _categoryRepository.GetAll();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnFilteredCategories()
        {
            // Arrange
            var category1 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category 1",
                ParentId = null
            };
            var category2 = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category 2",
                ParentId = null
            };

            await _dbContext.Categories.AddRangeAsync(category1, category2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _categoryRepository.GetAllAsync(x => x.Name.Contains("2"));

            // Assert
            Assert.Single(result);
            Assert.Equal(category2.Name, result.First().Name);
        }

        [Fact]
        public async Task Update_ShouldModifyCategory()
        {
            // Arrange
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                ParentId = null
            };

            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();

            // Act
            category.Name = "Updated Category";
            var result = await _categoryRepository.Update(category);
            await _dbContext.SaveChangesAsync();

            // Assert
            var updatedCategory = await _dbContext.Categories.FindAsync(category.Id);
            Assert.NotNull(updatedCategory);
            Assert.Equal("Updated Category", updatedCategory.Name);
        }
    }
}

