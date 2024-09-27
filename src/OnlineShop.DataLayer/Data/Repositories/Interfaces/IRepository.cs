using OnlineShop.DataLayer.Entities;
using System.Linq.Expressions;

namespace OnlineShop.DataLayer.Data.Repositories.Interfaces;

public interface IRepository<TEntity> where TEntity : IEntity
{
    Task Delete(Guid Id);
    Task<TEntity> Update(TEntity entity);
    Task<TEntity> Create(TEntity entity);
    Task<TEntity> Find(Guid id);
    Task<TEntity> Find(Guid id, Func<IQueryable<TEntity>, IQueryable<TEntity>>? include);
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> expression);
    IQueryable<TEntity> GetAll();
}
