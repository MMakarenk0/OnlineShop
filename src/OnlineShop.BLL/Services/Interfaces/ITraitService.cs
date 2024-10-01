using OnlineShop.BLL.Dtos.Create;
using OnlineShop.BLL.Dtos.Read;
using OnlineShop.BLL.Dtos.Update;

namespace OnlineShop.BLL.Services.Interfaces;

public interface ITraitService
{
    Task<IEnumerable<TraitDto>> GetAllAsync();
    Task<Guid> AddAsync(CreateTraitDto model);
    Task<Guid> UpdateAsync(UpdateTraitDto model);
    Task DeleteAsync(Guid id);
    Task<TraitDto> GetByIdAsync(Guid id);
}
