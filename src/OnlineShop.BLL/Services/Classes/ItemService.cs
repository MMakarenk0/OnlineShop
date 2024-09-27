using AutoMapper;
using DataLayer.Data.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.BLL.Services.Classes;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IBlobStorageService _blobStorageService;

    public ItemService(IUnitOfWork unitOfWork, IMapper mapper, IBlobStorageService blobStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _blobStorageService = blobStorageService;
    }
    public async Task<Guid> AddAsync(CreateItemDto model)
    {
        var itemRepository = _unitOfWork.ItemRepository;
        var itemImageRepository = _unitOfWork.ItemImageRepository;
        var categoryRepository = _unitOfWork.CategoryRepository;

        var item = _mapper.Map<Item>(model);

        if (model.CategoryIds != null && model.CategoryIds.Any())
        {
            foreach (var categoryId in model.CategoryIds)
            {
                var categoryExists = await categoryRepository.Find(categoryId);

                if (categoryExists == null)
                {
                    throw new Exception($"Category with Id {categoryId} not found.");
                }

                item.Categories.Add(categoryExists);
            }
        }

        if (model.ImageFiles != null && model.ImageFiles.Any())
        {
            foreach (var image in model.ImageFiles)
            {
                using var stream = image.OpenReadStream();
                var imageUrl = await _blobStorageService.UploadFileAsync(stream, image.FileName);

                var itemImage = new ItemImage
                {
                    Id = Guid.NewGuid(),
                    FileName = imageUrl,
                    ItemId = item.Id,
                };

                await itemImageRepository.Create(itemImage);
                item.Images.Add(itemImage);
            }
        }

        await itemRepository.Create(item);

        await _unitOfWork.SaveChangesAsync();

        return item.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var itemRepository = _unitOfWork.ItemRepository;

        var item = await itemRepository.Find(id);

        if (item == null)
            throw new Exception($"Item with Id {id} not found.");

        foreach (var image in item.Images)
        {
            await _blobStorageService.DeleteFileAsync(image.FileName);
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ItemDto>> GetAllAsync()
    {
        var itemRepository = _unitOfWork.ItemRepository;

        var items = await itemRepository.GetAll()
            .Include(i => i.Images)
            .Include(i => i.Categories)
                .ThenInclude(c => c.SubCategories)
            .ToListAsync();

        var itemDtos = _mapper.Map<IEnumerable<ItemDto>>(items);

        foreach (var itemDto in itemDtos)
        {
            var item = items.FirstOrDefault(x => x.Id == itemDto.Id);

            if (item != null)
            {
                itemDto.ImagesUrls = item.Images.Select(
                    image => _blobStorageService.
                    GetImageSasUri(image.FileName)).ToList();
            }
        }
        return itemDtos;
    }

    public async Task<ItemDto> GetByIdAsync(Guid id)
    {
        var itemRepository = _unitOfWork.ItemRepository;

        var item = await itemRepository.Find(id,
            include: i => i
            .Include(i => i.Images)
            .Include(i => i.Categories)
            .ThenInclude(c => c.SubCategories));

        if (item == null)
            throw new Exception($"Item with Id {id} not found.");

        var itemDto = _mapper.Map<ItemDto>(item);

        itemDto.ImagesUrls = item.Images.Select(image => _blobStorageService.GetImageSasUri(image.FileName)).ToList();

        return itemDto;
    }

    public async Task<Guid> UpdateAsync(UpdateItemDto model)
    {
        var itemRepository = _unitOfWork.ItemRepository;
        var itemImageRepository = _unitOfWork.ItemImageRepository;
        var categoryRepository = _unitOfWork.CategoryRepository;

        var item = await itemRepository.Find(model.Id,
            include: i => i
            .Include(i => i.Images)
            .Include(i => i.Categories));

        if (item == null)
            throw new Exception($"Item with Id {model.Id} not found.");

        _mapper.Map(model, item);

        await UpdateItemCategory(model, item);

        await UpdateItemImages(item, model.ImageFiles.ToList());

        var result = await itemRepository.Update(item);

        await _unitOfWork.SaveChangesAsync();

        return result.Id;
    }

    private async Task UpdateItemCategory(UpdateItemDto model, Item item)
    {
        var categoryRepository = _unitOfWork.CategoryRepository;

        item.Categories.Clear();

        if (model.CategoryIds != null && model.CategoryIds.Any())
        {
            foreach (var categoryId in model.CategoryIds)
            {
                var categoryExists = await categoryRepository.Find(categoryId);

                if (categoryExists == null)
                {
                    throw new Exception($"Category with Id {categoryId} not found.");
                }

                item.Categories.Add(categoryExists);
            }
        }
    }

    private async Task UpdateItemImages(Item item, List<IFormFile>? newImages)
    {
        if (newImages == null || !newImages.Any())
            return;

        var itemImageRepository = _unitOfWork.ItemImageRepository;

        // Deleting old images
        foreach (var image in item.Images.ToList())
        {
            await _blobStorageService.DeleteFileAsync(image.FileName);
            item.Images.Remove(image);
            await itemImageRepository.Delete(image.Id);
        }
        // Uploading new images
        foreach (var newImage in newImages)
        {
            using var stream = newImage.OpenReadStream();
            var imageName = await _blobStorageService.UploadFileAsync(stream, newImage.FileName);

            var itemImage = new ItemImage
            {
                Id = Guid.NewGuid(),
                FileName = imageName,
                ItemId = item.Id,
            };

            await itemImageRepository.Create(itemImage);
            item.Images.Add(itemImage);
        }
    }
}
