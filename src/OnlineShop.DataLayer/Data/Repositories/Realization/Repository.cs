using Microsoft.EntityFrameworkCore;
using OnlineShop.DataLayer.Data.Infrastructure;
using OnlineShop.DataLayer.Data.Repositories.Interfaces;
using OnlineShop.DataLayer.Entities;
using System.Linq.Expressions;

namespace DataLayer.Data.Repositories.Realization;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : IEntity
{
    protected readonly OnlineShopDbContext _dbContext;

    public Repository(OnlineShopDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TEntity> Create(TEntity entity)
    {
        var createdEntity = await _dbContext.Set<TEntity>().AddAsync(entity);
        return createdEntity.Entity;
    }

    public async Task Delete(Guid id)
    {
        var entity = _dbContext.Set<TEntity>().Find(id);
        if (entity != null)
        {
            _dbContext.Set<TEntity>().Remove(entity);
        }
    }

    public async Task<TEntity> Find(Guid id)
    {
        return await _dbContext.Set<TEntity>().FindAsync(id);
    }

    public IQueryable<TEntity> GetAll()
    {
        return _dbContext.Set<TEntity>();
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await _dbContext.Set<TEntity>().Where(expression).ToListAsync();
    }

    public async Task<TEntity> Update(TEntity entity)
    {
        var updatedEntity = _dbContext.Set<TEntity>().Update(entity);
        return updatedEntity.Entity;
    }
}