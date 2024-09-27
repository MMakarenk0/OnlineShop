using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;

namespace OnlineShop.BLL.Services.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<Guid> AddAsync(CreateCategoryDto model);
    Task<Guid> UpdateAsync(UpdateCategoryDto model);
    Task DeleteAsync(Guid id);
    Task<CategoryDto> GetByIdAsync(Guid id);
}
