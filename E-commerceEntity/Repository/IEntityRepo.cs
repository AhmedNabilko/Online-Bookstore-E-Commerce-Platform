using E_commerceEntity.DataBase;
using E_commerceEntity.Entity;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace E_commerceEntity.Repository
{
    public interface IEntityRepo<T> where T : class, IEntity
    {

        // Create
        public void Add(T element);
        // Update
        public void Update(T element);
        //Delete
        public void Delete(T element);

        //GetbyId
        public T GetById(int id);
        // GetAll
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] preloads);
        // FindAll
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> condition, params Expression<Func<T, object>>[] preloads);
        public IEnumerable<T> GetPaginated(int pageNumber, int pageSize, Expression<Func<T, bool>> condition = null, params Expression<Func<T, object>>[] preloads);
        public int GetTotalCount(Expression<Func<T, bool>> condition = null);
        public IQueryable<T> Includes(IQueryable<T> query, params Expression<Func<T, object>>[] preloads);
    }
}
