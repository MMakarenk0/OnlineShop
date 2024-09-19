using DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Entities;

namespace DataLayer.Data.Repositories.Realization;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(OnlineShopDbContext dbContext) : base(dbContext)
    {
    }
}