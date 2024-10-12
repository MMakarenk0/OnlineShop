using DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Entities;

namespace DataLayer.Data.Repositories.Realization;

public class ItemRepository : Repository<Item>, IItemRepository
{
    public ItemRepository(OnlineShopDbContext dbContext) : base(dbContext)
    {
    }
}
