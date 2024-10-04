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
        var traitRepository = _unitOfWork.TraitRepository;
        var itemTraitRepository = _unitOfWork.ItemTraitRepository;

        var item = _mapper.Map<Item>(model);

        if (model.CategoryIds != null && model.CategoryIds.Any())
        {
            foreach (var categoryId in model.CategoryIds)
            {
                var categoryExists = await categoryRepository.Find(categoryId);

                if (categoryExists == null)
                {
                    throw new KeyNotFoundException($"Category with Id {categoryId} not found.");
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
        if (model.TraitValues != null && model.TraitValues.Any())
        {
            foreach (var traitValueDto in model.TraitValues)
            {
                var trait = await traitRepository.Find(traitValueDto.TraitId);

                if (trait == null)
                {
                    throw new KeyNotFoundException($"Trait with Id {traitValueDto.TraitId} not found.");
                }

                var itemTrait = new ItemTrait
                {
                    Id = Guid.NewGuid(),
                    ItemId = item.Id,
                    TraitId = trait.Id,
                    Value = traitValueDto.Value
                };

                await itemTraitRepository.Create(itemTrait);
                item.ItemTraits.Add(itemTrait);
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
            throw new KeyNotFoundException($"Item with Id {id} not found.");

        foreach (var image in item.Images)
        {
            await _blobStorageService.DeleteFileAsync(image.FileName);
        }

        await itemRepository.Delete(id);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<ItemDto>> GetAllAsync()
    {
        var itemRepository = _unitOfWork.ItemRepository;

        var items = await itemRepository.GetAll()
            .Include(i => i.Images)
            .Include(i => i.Categories)
                .ThenInclude(c => c.SubCategories)
            .Include(i => i.ItemTraits)
                .ThenInclude(it => it.Trait)
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
                .ThenInclude(c => c.SubCategories)
            .Include(i => i.ItemTraits)
                .ThenInclude(it => it.Trait));

        if (item == null)
            throw new KeyNotFoundException($"Item with Id {id} not found.");

        var itemDto = _mapper.Map<ItemDto>(item);

        itemDto.ImagesUrls = item.Images.Select(image => _blobStorageService.GetImageSasUri(image.FileName)).ToList();

        return itemDto;
    }
    public async Task<IEnumerable<ItemDto>> GetByFilters(ItemFilterDto filters)
    {
        var itemRepository = _unitOfWork.ItemRepository;

        var itemsQuery = itemRepository.GetAll()
            .Include(i => i.Categories)
            .Include(i => i.ItemTraits)
            .Include(i => i.Images)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            itemsQuery = itemsQuery.Where(i => i.Name.Contains(filters.Name));
        }

        if (filters.MinPrice.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.Price >= filters.MinPrice.Value);
        }

        if (filters.MaxPrice.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.Price <= filters.MaxPrice.Value);
        }

        if (filters.MinQuantityInStock.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.QuantityInStock >= filters.MinQuantityInStock.Value);
        }

        if (filters.MaxQuantityInStock.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.QuantityInStock <= filters.MaxQuantityInStock.Value);
        }

        if (filters.CategoryId.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.Categories.Any(c => c.Id == filters.CategoryId.Value));
        }

        if (filters.CreatedAfter.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.CreatedAt >= filters.CreatedAfter.Value);
        }

        if (filters.CreatedBefore.HasValue)
        {
            itemsQuery = itemsQuery.Where(i => i.CreatedAt <= filters.CreatedBefore.Value);
        }

        if (filters.TraitIds != null && filters.TraitIds.Any())
        {
            itemsQuery = itemsQuery
                .Where(i => i.ItemTraits.Any(it => filters.TraitIds.Contains(it.TraitId)));
        }

        var filteredItems = await itemsQuery.ToListAsync();

        return _mapper.Map<IEnumerable<ItemDto>>(filteredItems);
    }

    public async Task<Guid> UpdateAsync(UpdateItemDto model)
    {
        var itemRepository = _unitOfWork.ItemRepository;
        var itemImageRepository = _unitOfWork.ItemImageRepository;
        var categoryRepository = _unitOfWork.CategoryRepository;

        var item = await itemRepository.Find(model.Id,
            include: i => i
            .Include(i => i.Images)
            .Include(i => i.Categories)
            .Include(i => i.ItemTraits));

        if (item == null)
            throw new KeyNotFoundException($"Item with Id {model.Id} not found.");

        _mapper.Map(model, item);

        await UpdateItemCategory(model, item);
        await UpdateItemImages(item, model.ImageFiles.ToList());
        await UpdateItemTraits(model, item);

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
                    throw new KeyNotFoundException($"Category with Id {categoryId} not found.");
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
    private async Task UpdateItemTraits(UpdateItemDto model, Item item)
    {
        var itemTraitRepository = _unitOfWork.ItemTraitRepository;
        var traitRepository = _unitOfWork.TraitRepository;

        // Delete existing traits that are not passed to the model
        var existingTraitIds = item.ItemTraits.Select(t => t.TraitId).ToList();
        var newTraitIds = model.TraitValues?.Select(t => t.TraitId).ToList() ?? new List<Guid>();

        // Remove those traits that are missing from the updated data
        var traitsToRemove = item.ItemTraits
            .Where(t => !newTraitIds.Contains(t.TraitId))
            .ToList();

        foreach (var traitToRemove in traitsToRemove)
        {
            item.ItemTraits.Remove(traitToRemove);
            await itemTraitRepository.Delete(traitToRemove.Id);
        }
        // Update or add new traits values
        if (model.TraitValues != null && model.TraitValues.Any())
        {
            foreach (var traitValueDto in model.TraitValues)
            {
                var existingItemTrait = item.ItemTraits
                    .FirstOrDefault(t => t.TraitId == traitValueDto.TraitId);

                if (existingItemTrait != null)
                {
                    existingItemTrait.Value = traitValueDto.Value;
                    await itemTraitRepository.Update(existingItemTrait);
                }
                else
                {
                    var trait = await traitRepository.Find(traitValueDto.TraitId);
                    if (trait == null)
                    {
                        throw new KeyNotFoundException($"Trait with Id {traitValueDto.TraitId} not found.");
                    }

                    var newItemTrait = new ItemTrait
                    {
                        Id = Guid.NewGuid(),
                        ItemId = item.Id,
                        TraitId = trait.Id,
                        Value = traitValueDto.Value
                    };

                    await itemTraitRepository.Create(newItemTrait);
                    item.ItemTraits.Add(newItemTrait);
                }
            }
        }
    }
}
