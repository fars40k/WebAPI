using System;
using System.Collections.Generic;

namespace WebAPI_App.Data.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        TEntity FindByID(Guid id);
        IEnumerable<TEntity> FindAll();

        void Insert(TEntity entity);
        void Update(TEntity entityToUpdate);

        void Delete(TEntity entityToDelete);
        void Delete(Guid id);

        void SaveChanges();

    }
}
