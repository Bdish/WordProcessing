using DALWordProc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DALWordProc.Repository.Implementations
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        DbContext _context;
        DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> Get()
        {
            return _dbSet./*AsNoTracking().*/ToList();
        }

        public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
        {
            return _dbSet./*AsNoTracking().*/Where(predicate).ToList();
        }
        public TEntity FindById(int id)
        {
            return _dbSet.Find(id);
        }
        public virtual TEntity FindByItem(TEntity item)
        {
            return item;
        }

        public void Create(TEntity item)
        {
            
            _dbSet.Add(item);
            _context.SaveChanges();
        }

        public virtual void Update(TEntity item)
        {

            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Remove(TEntity item)
        {
            _dbSet.Remove(item);
            _context.SaveChanges();
        }

        public void ExecuteSQLExpression(string SQLExpression)
        {
            _context.Database.ExecuteSqlCommand(SQLExpression);
        }
    }
}
