using E_commerceEntity.DataBase;
using E_commerceEntity.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace E_commerceEntity.Repository
{
    public class Repository<T> : IEntityRepo<T> where T : class, IEntity
    {
        E_CommerceContext Context { get; set; }
        DbSet<T> Set { get; set; }
        public Repository(E_CommerceContext Context)
        {
            this.Context = Context;
            this.Set = Context.Set<T>();
        }

        public void Add(T element)
        {
            Set.Add(element);
        }

        public void Delete(T element)
        {
            Set.Remove(element);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] preloads)
        {
            if (condition == null)
                return Includes(Set, preloads).ToList();
            return Includes(Set, preloads).Where(condition).ToList();

        }

        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] preloads)
        {
            return Includes(Set, preloads).ToList();
        }

        public IEnumerable<T> GetPaginated(int pageNumber, int pageSize, Expression<Func<T, bool>> condition = null, params Expression<Func<T, object>>[] preloads)
        {
            var query = Includes(Set, preloads);
            if (condition != null)
                query = query.Where(condition);

            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        public int GetTotalCount(Expression<Func<T, bool>> condition = null)
        {
            if (condition != null)
                return Set.Count(condition);
            return Set.Count();
        }

        public T GetById(int id)
        {
            return Set.Find(id);

        }
        public IQueryable<T> Includes(IQueryable<T> query, params Expression<Func<T, object>>[] preloads)
        {
            if (preloads != null)
            {
                foreach (var preload in preloads)
                {
                    query = query.Include(preload);
                }
            }
            return query;
        }

        public void Update(T element)
        {
            Set.Update(element);
        }
    }
}
