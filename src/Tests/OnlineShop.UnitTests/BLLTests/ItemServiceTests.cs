using AutoMapper;
using DataLayer.Data.Infrastructure;
using DataLayer.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
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
        var itemDtos = new List<ItemDto>
        {
            new ItemDto { Id = items[0].Id, Name = "Item 1" },
            new ItemDto { Id = items[1].Id, Name = "Item 2" }
        };
        _itemRepository.GetAll().Returns(items.AsQueryable());
        _mapper.Map<IEnumerable<ItemDto>>(items).Returns(itemDtos);

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

        _itemRepository.Find(itemId).Returns(Task.FromResult(item));
        _mapper.Map<ItemDto>(item).Returns(itemDto);

        // Act
        var result = await _itemService.GetByIdAsync(itemId);

        // Assert
        Assert.Equal(itemDto.Id, result.Id);
        await _itemRepository.Received(1).Find(itemId);
    }

}