using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BookStore.Data
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal BookStoreContext _ctx;
        internal IMapper _mapper;

        public GenericRepository(BookStoreContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _ctx.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            
            if (orderBy != null)
            {
                return orderBy(query).AsQueryable();
            }
            else
            {
                return query;
            }
        }

        public IQueryable<TEntity> GetAll()
        {
            return _ctx.Set<TEntity>().AsQueryable();
        }

        public virtual TEntity GetById(Expression<Func<TEntity, bool>> predicate)
        {
            return _ctx.Set<TEntity>().FirstOrDefault(predicate);
        }

        public virtual void Insert(TEntity entity)
        {
            _ctx.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _ctx.Set<TEntity>().Attach(entityToUpdate);
            _ctx.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            TEntity entityToDelete = _ctx.Set<TEntity>().FirstOrDefault(predicate);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_ctx.Entry(entityToDelete).State == EntityState.Detached)
            {
                _ctx.Set<TEntity>().Attach(entityToDelete);
            }
            _ctx.Set<TEntity>().Remove(entityToDelete);
        }
    }
}
