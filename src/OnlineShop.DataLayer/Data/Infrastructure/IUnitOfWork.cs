using DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;

namespace DataLayer.Data.Infrastructure;

public interface IUnitOfWork
{
    IItemRepository ItemRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IItemImageRepository ItemImageRepository { get; }
    ITraitRepository TraitRepository { get; }
    IItemTraitRepository ItemTraitRepository { get; }

    Task SaveChangesAsync();
}
