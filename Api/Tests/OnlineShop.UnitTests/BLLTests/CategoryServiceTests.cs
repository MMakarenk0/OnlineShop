using AutoMapper;
using DataLayer.Data.Infrastructure;
using DataLayer.Data.Repositories.Interfaces;
using MockQueryable;
using MockQueryable.NSubstitute;
using NSubstitute;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;
using OnlineShop.BLL.Services.Classes;
using OnlineShop.BLL.Services.Interfaces;
using OnlineShop.DataLayer.Entities;
using System.Collections.Generic;

namespace UnitTests.BLLTests;

public class CategoryServiceTests
{
    private readonly ICategoryService _categoryService;
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();

    public CategoryServiceTests()
    {
        _unitOfWork.CategoryRepository.Returns(_categoryRepository);
        _categoryService = new CategoryService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCategory_WhenDataIsValid()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Electronics", ParentId = null };
        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics" };

        _mapper.Map<Category>(createDto).Returns(category);
        _categoryRepository.Create(category).Returns(category);

        // Act
        var result = await _categoryService.AddAsync(createDto);

        // Assert
        Assert.Equal(category.Id, result);
        await _categoryRepository.Received(1).Create(category);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task AddAsync_ShouldThrowException_WhenParentCategoryNotFound()
    {
        // Arrange
        var createDto = new CreateCategoryDto { Name = "Laptops", ParentId = Guid.NewGuid() };

        _categoryRepository.Find(createDto.ParentId.Value).Returns((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _categoryService.AddAsync(createDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCategory_WhenIdIsValid()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        await _categoryService.DeleteAsync(categoryId);

        // Assert
        await _categoryRepository.Received(1).Delete(categoryId);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCategories_WithSubCategoriesAndTraits()
    {
        // Arrange
        var categoriesQuery = new List<Category>
        {
            new Category { Id = Guid.NewGuid(), Name = "Electronics" }
        }.BuildMock();

        _categoryRepository.GetAll().Returns(categoriesQuery);

        var categoryDtos = new List<CategoryDto> { new CategoryDto { Name = "Electronics" } };

        _mapper.Map<IEnumerable<CategoryDto>>(Arg.Any<List<Category>>()).Returns(categoryDtos);

        // Act
        var result = await _categoryService.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Electronics", result.First().Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCategory_WhenIdIsValid()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category { Id = categoryId, Name = "Electronics" };
        var categoryDto = new CategoryDto { Id = categoryId, Name = "Electronics" };

        _categoryRepository.Find(categoryId, Arg.Any<Func<IQueryable<Category>, IQueryable<Category>>>())
            .Returns(category);
        _mapper.Map<CategoryDto>(category).Returns(categoryDto);

        // Act
        var result = await _categoryService.GetByIdAsync(categoryId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryDto.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCategory_WhenDataIsValid()
    {
        // Arrange
        var updateDto = new UpdateCategoryDto { Id = Guid.NewGuid(), Name = "Updated Electronics" };
        var category = new Category { Id = updateDto.Id, Name = "Electronics" };

        _categoryRepository.Find(updateDto.Id).Returns(category);

        // Act
        var result = await _categoryService.UpdateAsync(updateDto);

        // Assert
        Assert.Equal(updateDto.Id, result);
        _mapper.Received(1).Map(updateDto, category);
        await _unitOfWork.Received(1).SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenCategoryNotFound()
    {
        // Arrange
        var updateDto = new UpdateCategoryDto { Id = Guid.NewGuid(), Name = "Non-existent" };

        _categoryRepository.Find(updateDto.Id).Returns((Category)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _categoryService.UpdateAsync(updateDto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenCyclicReferenceDetected()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var parentId = Guid.NewGuid();
        var updateDto = new UpdateCategoryDto { Id = categoryId, ParentId = parentId };
        var category = new Category { Id = categoryId };
        var parentCategory = new Category { Id = parentId, ParentCategory = category };

        _categoryRepository.Find(updateDto.Id).Returns(category);
        _categoryRepository.Find(parentId).Returns(parentCategory);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _categoryService.UpdateAsync(updateDto));
    }
}
