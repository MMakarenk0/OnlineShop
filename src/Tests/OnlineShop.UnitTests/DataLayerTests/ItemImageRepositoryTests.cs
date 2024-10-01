using DataLayer.Data.Repositories.Realization;
using Microsoft.EntityFrameworkCore;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.UnitTests.DataLayerTests
{
    public class ItemImageRepositoryTests
    {
        private readonly ItemImageRepository _itemImageRepository;
        private readonly OnlineShopDbContext _dbContext;

        public ItemImageRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<OnlineShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new OnlineShopDbContext(options);
            _itemImageRepository = new ItemImageRepository(_dbContext);
        }

        [Fact]
        public async Task Create_ShouldAddItemImage()
        {
            // Arrange
            var itemImage = new ItemImage
            {
                FileName = "test_image.jpg",
                ItemId = Guid.NewGuid()
            };

            // Act
            var result = await _itemImageRepository.Create(itemImage);
            await _dbContext.SaveChangesAsync();

            // Assert
            var createdImage = await _dbContext.Images.FindAsync(result.Id);
            Assert.NotNull(createdImage);
            Assert.Equal(itemImage.FileName, createdImage.FileName);
        }

        [Fact]
        public async Task Delete_ShouldRemoveItemImage()
        {
            // Arrange
            var itemImage = new ItemImage
            {
                Id = Guid.NewGuid(),
                FileName = "test_image.jpg",
                ItemId = Guid.NewGuid()
            };

            await _dbContext.Images.AddAsync(itemImage);
            await _dbContext.SaveChangesAsync();

            // Act
            await _itemImageRepository.Delete(itemImage.Id);
            await _dbContext.SaveChangesAsync();

            // Assert
            var deletedImage = await _dbContext.Images.FindAsync(itemImage.Id);
            Assert.Null(deletedImage);
        }

        [Fact]
        public async Task Find_ShouldReturnItemImage()
        {
            // Arrange
            var itemImage = new ItemImage
            {
                Id = Guid.NewGuid(),
                FileName = "test_image.jpg",
                ItemId = Guid.NewGuid()
            };

            await _dbContext.Images.AddAsync(itemImage);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _itemImageRepository.Find(itemImage.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(itemImage.FileName, result.FileName);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllImages()
        {
            // Arrange
            var image1 = new ItemImage
            {
                Id = Guid.NewGuid(),
                FileName = "image1.jpg",
                ItemId = Guid.NewGuid()
            };
            var image2 = new ItemImage
            {
                Id = Guid.NewGuid(),
                FileName = "image2.jpg",
                ItemId = Guid.NewGuid()
            };

            await _dbContext.Images.AddRangeAsync(image1, image2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = _itemImageRepository.GetAll();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Update_ShouldModifyItemImage()
        {
            // Arrange
            var itemImage = new ItemImage
            {
                Id = Guid.NewGuid(),
                FileName = "test_image.jpg",
                ItemId = Guid.NewGuid()
            };

            await _dbContext.Images.AddAsync(itemImage);
            await _dbContext.SaveChangesAsync();

            // Act
            itemImage.FileName = "updated_image.jpg";
            var result = await _itemImageRepository.Update(itemImage);
            await _dbContext.SaveChangesAsync();

            // Assert
            var updatedImage = await _dbContext.Images.FindAsync(itemImage.Id);
            Assert.NotNull(updatedImage);
            Assert.Equal("updated_image.jpg", updatedImage.FileName);
        }
    }
}
