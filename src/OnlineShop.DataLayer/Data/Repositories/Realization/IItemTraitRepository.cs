using DataLayer.Data.Repositories.Realization;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Repositories.Realization;

public class ItemTraitRepository : Repository<ItemTrait>, IItemTraitRepository
{
    public ItemTraitRepository(OnlineShopDbContext dbContext) : base(dbContext)
    {
    }
    // Overloading methonds for composite keys
    public async Task<ItemTrait> Find(Guid itemId, Guid traitId)
    {
        return await _dbContext.Set<ItemTrait>().FindAsync(itemId, traitId);
    }

    public async Task Delete(Guid itemId, Guid traitId)
    {
        var entity = _dbContext.Set<ItemTrait>().Find(itemId, traitId);
        if (entity != null)
        {
            _dbContext.Set<ItemTrait>().Remove(entity);
        }
    }
}
