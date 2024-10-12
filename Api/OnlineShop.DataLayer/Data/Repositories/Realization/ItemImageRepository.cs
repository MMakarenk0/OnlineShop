using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace DataLayer.Data.Repositories.Realization;

public class ItemImageRepository : Repository<ItemImage>, IItemImageRepository
{
    public ItemImageRepository(OnlineShopDbContext dbContext) : base(dbContext)
    {
    }
}
