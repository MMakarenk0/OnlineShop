using Microsoft.AspNetCore.Http;
using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;

namespace OnlineShop.BLL.Services.Interfaces;

public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetAllAsync();
    Task<Guid> AddAsync(CreateItemDto model);
    Task<Guid> UpdateAsync(UpdateItemDto model);
    Task DeleteAsync(Guid id);
    Task<ItemDto> GetByIdAsync(Guid id);
}
