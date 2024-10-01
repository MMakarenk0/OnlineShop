using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Repositories.Interfaces;

public interface IItemTraitRepository : IRepository<ItemTrait>
{
    Task<ItemTrait> Find(Guid itemId, Guid traitId);
    Task Delete(Guid itemId, Guid traitId);
}
