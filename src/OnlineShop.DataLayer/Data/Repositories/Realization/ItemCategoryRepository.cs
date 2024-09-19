using DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Entities;

namespace DataLayer.Data.Repositories.Realization;

public class ItemCategoryRepository : Repository<ItemCategory>, IItemCategoryRepository
{
    public ItemCategoryRepository(OnlineShopDbContext dbContext) : base(dbContext)
    {
    }
    // Overloading methonds for composite keys
    public async Task<ItemCategory> Find(Guid itemId, Guid categoryId)
    {
        return await _dbContext.Set<ItemCategory>().FindAsync(itemId, categoryId);
    }

    public async Task Delete(Guid itemId, Guid categoryId)
    {
        var entity = _dbContext.Set<ItemCategory>().Find(itemId, categoryId);
        if (entity != null)
        {
            _dbContext.Set<ItemCategory>().Remove(entity);
        }
    }
}
