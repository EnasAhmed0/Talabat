using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repository;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreDbContext _dbContext;
        private Hashtable _repositories;
        public UnitOfWork(StoreDbContext dbContext)
        {
            _dbContext = dbContext;
            _repositories = new Hashtable();
        }
        public async Task<int> CompleteAsync()
        => await _dbContext.SaveChangesAsync();

        public async ValueTask DisposeAsync()
        => await _dbContext.DisposeAsync();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var Repo = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(type, Repo);

            }
            return /*(IGenericRepository<TEntity>)*/ _repositories[type] as IGenericRepository<TEntity>;
        }


    }
}
