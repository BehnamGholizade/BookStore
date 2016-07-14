using System;
using System.Linq;
using System.Linq.Expressions;

namespace BookStore.Data
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        void Delete(TEntity entityToDelete);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");
        IQueryable<TEntity> GetAll();
        TEntity GetById(Expression<Func<TEntity, bool>> predicate);
        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);
    }
}