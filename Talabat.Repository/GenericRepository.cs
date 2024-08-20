using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repository;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreDbContext _dbcontext;

        public GenericRepository(StoreDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        #region Without Specification
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            if (typeof(T) == typeof(Product))
            {
                return (IReadOnlyList<T>)await _dbcontext.Products.Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();
            }
            return await _dbcontext.Set<T>().ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            //if (typeof(T) == typeof(Product))
            //{
            //    return (T) await _dbcontext.Products.Where(P => P.Id == id).Include(P => P.ProductBrand).Include(P => P.ProductType);
            //}
            return await _dbcontext.Set<T>().FindAsync(id);
        }
        #endregion


        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            //return await SpecificationEvaluator<T>.GetQuery(_dbcontext.Set<T>(), spec).ToListAsync();
            return await ApplySpecifications(spec).ToListAsync();
        }
        public async Task<T> GetEntityWithSpecAsync(ISpecifications<T> spec)
        {
            //return await SpecificationEvaluator<T>.GetQuery(_dbcontext.Set<T>(), spec).FirstOrDefaultAsync();
            return await ApplySpecifications(spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecifications(ISpecifications<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbcontext.Set<T>(), spec);
        }

        public async Task<int> GetCountWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplySpecifications(spec).CountAsync();
        }

        public async Task AddAsync(T item)
        => await _dbcontext.Set<T>().AddAsync(item);

        public void Delete(T item)
        => _dbcontext.Set<T>().Remove(item);

        public void Update(T item)
        => _dbcontext.Set<T>().Update(item);
    }
}
