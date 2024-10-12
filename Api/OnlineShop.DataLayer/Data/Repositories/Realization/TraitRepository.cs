using DataLayer.Data.Repositories.Realization;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Entities;

namespace OnlineShop.DataLayer.Data.Repositories.Realization;

public class TraitRepository : Repository<Trait>, ITraitRepository
{
    public TraitRepository(OnlineShopDbContext dbContext) : base(dbContext)
    {
    }
}
