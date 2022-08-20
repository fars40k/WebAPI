using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using WebAPI_App.Data.Interfaces;

namespace WebAPI_App.Data
{
    public class BaseRepository<TEntity> : IRepository<TEntity>  where TEntity : class
    {
        private WinTaskContext _context;
        internal DbSet<TEntity> _dbSet;

        private bool disposed = false;

        public BaseRepository(WinTaskContext context)
        {
            this._context = context;
            this._dbSet = context.Set<TEntity>();
        }

        public virtual TEntity FindByID(Guid id)
        {
            return _dbSet.Find(id);
        }

        public IEnumerable<TEntity> FindAll()
        {
            return _dbSet.AsNoTracking().ToList();
        }

        public virtual void Insert(TEntity entity)
        {         
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public virtual void Delete(Guid id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            _dbSet.Remove(entityToDelete);
            _context.SaveChanges();
        }

        public void Delete(TEntity entityToDelete)
        {
            _context.Entry(entityToDelete).State = EntityState.Deleted;
            _dbSet.Remove(entityToDelete);
            _context.SaveChanges();
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {

            }
            disposed = true;
        }

        ~BaseRepository()
        {
            Dispose(false);
        }
    }

}

