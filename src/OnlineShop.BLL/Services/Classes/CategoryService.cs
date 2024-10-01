using AutoMapper;
using DataLayer.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.BLL.Services.Classes;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Guid> AddAsync(CreateCategoryDto model)
    {
        var categoryRepository = _unitOfWork.CategoryRepository;

        var category = _mapper.Map<Category>(model);

        if (model.ParentId.HasValue)
        {
            var parent = await categoryRepository.Find(model.ParentId.Value);

            if (parent == null)
                throw new KeyNotFoundException($"Category with Id {model.ParentId.Value} not found.");

            category.ParentCategory = parent;
        }

        var result = await categoryRepository.Create(category);

        await _unitOfWork.SaveChangesAsync();

        return result.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var categoryRepository = _unitOfWork.CategoryRepository;

        await categoryRepository.Delete(id);

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categoryRepository = _unitOfWork.CategoryRepository;

        var categories = await categoryRepository.GetAll()
            .Include(c => c.SubCategories)
            .Include(c => c.Traits)
            .ToListAsync();

        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> GetByIdAsync(Guid id)
    {
        var categoryRepository = _unitOfWork.CategoryRepository;

        var category = await categoryRepository.Find(id,
            include: i => i
                .Include(i => i.SubCategories)
                .Include(c => c.Traits));

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<Guid> UpdateAsync(UpdateCategoryDto model)
    {
        var categoryRepository = _unitOfWork.CategoryRepository;

        var category = await categoryRepository.Find(model.Id);
        if (category == null)
            throw new KeyNotFoundException($"Category with Id {model.Id} not found.");

        _mapper.Map(model, category);

        if (model.ParentId.HasValue)
        {
            var parent = await categoryRepository.Find(model.ParentId.Value);
            category.ParentCategory = parent;
        }

        await categoryRepository.Update(category);

        await _unitOfWork.SaveChangesAsync();

        return category.Id;
    }
}
