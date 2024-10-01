using DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;

namespace DataLayer.Data.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly OnlineShopDbContext _dbContext;
    public IItemRepository ItemRepository { get; }
    public ICategoryRepository CategoryRepository { get; }
    public IItemImageRepository ItemImageRepository { get; }
    public ITraitRepository TraitRepository { get; }
    public IItemTraitRepository ItemTraitRepository { get; }

    public UnitOfWork(
        OnlineShopDbContext dbContext,
        IItemRepository itemRepository,
        ICategoryRepository categoryRepository,
        IItemImageRepository itemImageRepository,
        ITraitRepository traitRepository,
        IItemTraitRepository itemTraitRepository)
    {
        _dbContext = dbContext;
        ItemRepository = itemRepository;
        CategoryRepository = categoryRepository;
        ItemImageRepository = itemImageRepository;
        TraitRepository = traitRepository;
        ItemTraitRepository = itemTraitRepository;
    }
    public async Task SaveChangesAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}
