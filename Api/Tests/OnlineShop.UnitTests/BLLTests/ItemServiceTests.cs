using AutoMapper;
using DataLayer.Data.Infrastructure;
using DataLayer.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using MockQueryable.NSubstitute;
using NSubstitute;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Classes;
using OnlineShop.BLL.Services.Interfaces;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace UnitTests.BLLTests;

public class ItemServiceTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IItemRepository _itemRepository;
    private readonly IItemImageRepository _itemImageRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITraitRepository _traitRepository;
    private readonly IItemTraitRepository _itemTraitRepository;
    private readonly ItemService _itemService;

    public ItemServiceTests()
    {
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _mapper = Substitute.For<IMapper>();
        _blobStorageService = Substitute.For<IBlobStorageService>();
        _itemRepository = Substitute.For<IItemRepository>();
        _itemImageRepository = Substitute.For<IItemImageRepository>();
        _categoryRepository = Substitute.For<ICategoryRepository>();
        _traitRepository = Substitute.For<ITraitRepository>();
        _itemTraitRepository = Substitute.For<IItemTraitRepository>();

        _unitOfWork.ItemRepository.Returns(_itemRepository);
        _unitOfWork.ItemImageRepository.Returns(_itemImageRepository);
        _unitOfWork.CategoryRepository.Returns(_categoryRepository);
        _unitOfWork.TraitRepository.Returns(_traitRepository);
        _unitOfWork.ItemTraitRepository.Returns(_itemTraitRepository);

        _itemService = new ItemService(_unitOfWork, _mapper, _blobStorageService);
    }

    [Fact]
    public async Task AddAsync_ShouldAddItemWithImagesAndReturnId()
    {
        // Arrange
        var model = new CreateItemDto
        {
            Name = "New Item",
            ImageFiles = new List<IFormFile>
            {
                Substitute.For<IFormFile>(),
                Substitute.For<IFormFile>()
            }
        };
        var item = new Item { Id = Guid.NewGuid(), Name = "New Item" };
        _mapper.Map<Item>(model).Returns(item);

        // Act
        var result = await _itemService.AddAsync(model);

        // Assert
        Assert.Equal(item.Id, result);
        await _itemRepository.Received(1).Create(item);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteItemAndImages()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item
        {
            Id = itemId,
            Images = new List<ItemImage>
            {
                new ItemImage { FileName = "image1.jpg" },
                new ItemImage { FileName = "image2.jpg" }
            }
        };
        _itemRepository.Find(itemId).Returns(item);

        // Act
        await _itemService.DeleteAsync(itemId);

        // Assert
        await _itemRepository.Received(1).Delete(itemId);
        await _blobStorageService.Received(1).DeleteFileAsync("image1.jpg");
        await _blobStorageService.Received(1).DeleteFileAsync("image2.jpg");
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllItemsWithImages()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Id = Guid.NewGuid(), Name = "Item 1", Images = new List<ItemImage>() },
            new Item { Id = Guid.NewGuid(), Name = "Item 2", Images = new List<ItemImage>() }
        };
        var mock = items.BuildMock();
        var itemDtos = new List<ItemDto>
        {
            new ItemDto { Id = items[0].Id, Name = "Item 1" },
            new ItemDto { Id = items[1].Id, Name = "Item 2" }
        };
        _itemRepository.GetAll().Returns(mock);
        _mapper.Map<IEnumerable<ItemDto>>(default).ReturnsForAnyArgs(itemDtos);

        // Act
        var result = await _itemService.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _itemRepository.Received(1).GetAll();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnItemById()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var item = new Item { Id = itemId, Name = "Test Item" };
        var itemDto = new ItemDto { Id = itemId, Name = "Test Item" };

        _itemRepository.Find(item.Id, Arg.Any<Func<IQueryable<Item>, IQueryable<Item>>>()).Returns(Task.FromResult(item));
        _mapper.Map<ItemDto>(item).Returns(itemDto);

        // Act
        var result = await _itemService.GetByIdAsync(itemId);

        // Assert
        Assert.Equal(itemDto.Id, result.Id);
        await _itemRepository.Received(1).Find(itemId, Arg.Any<Func<IQueryable<Item>, IQueryable<Item>>>());
    }

    [Fact]
    public async Task GetByFilters_ShouldReturnFilteredItems()
    {
        // Arrange
        var filters = new ItemFilterDto
        {
            Name = "Test",
            MinPrice = 10,
            MaxPrice = 50,
            MinQuantityInStock = 5,
            MaxQuantityInStock = 20,
        };

        var items = new List<Item>
        {
            new Item {
                Id = Guid.NewGuid(),
                Name = "Test Item",
                Price = 20,
                QuantityInStock = 10,
                CreatedAt = DateTime.UtcNow
            }
        };

        var itemDtos = new List<ItemDto>
        {
            new ItemDto
            {
                Id = items[0].Id,
                Name = items[0].Name,
                Price = items[0].Price,
                QuantityInStock = items[0].QuantityInStock,
                CreatedAt = items[0].CreatedAt
            }
        };

        var mockQuery = items.BuildMock().AsQueryable();
        _mapper.Map<IEnumerable<ItemDto>>(default).ReturnsForAnyArgs(itemDtos);
        _itemRepository.GetAll().Returns(mockQuery);

        // Act
        var result = await _itemService.GetByFilters(filters);

        // Assert
        Assert.Single(result);
        Assert.Equal(items.First().Id, result.First().Id);
        _itemRepository.Received(1).GetAll();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateItemAndReturnId()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var existingItem = new Item
        {
            Id = itemId,
            Name = "Old Item",
            Images = new List<ItemImage> { new ItemImage { FileName = "old_image.jpg" } },
        };

        var model = new UpdateItemDto
        {
            Id = itemId,
            Name = "Updated Item",
            ImageFiles = new List<IFormFile> { Substitute.For<IFormFile>() },
        };

        _itemRepository.Find(itemId, Arg.Any<Func<IQueryable<Item>, IQueryable<Item>>>())
            .Returns(existingItem);
        _mapper.Map(Arg.Any<UpdateItemDto>(), Arg.Any<Item>()).Returns(existingItem);
        var updatedItem = existingItem;
        _itemRepository.Update(Arg.Any<Item>()).Returns(updatedItem);

        // Act
        var result = await _itemService.UpdateAsync(model);

        // Assert
        Assert.Equal(itemId, result);
        await _itemRepository.Received(1).Update(existingItem);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

}