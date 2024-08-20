using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Core.Repository
{
    public interface IGenericRepository<T> where T:BaseEntity
    {
        #region Without Specifications
        //Get All Products
        Task<IReadOnlyList<T>> GetAllAsync();
        // Get by Id
        Task<T> GetByIdAsync(int id);

        #endregion

        #region With Specification
        //Get All With Specification
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec);
        //Get By Id With Specification
        Task<T> GetEntityWithSpecAsync(ISpecifications<T> spec);
        #endregion

        Task<int> GetCountWithSpecAsync(ISpecifications<T> spec);
        
        
        // Add
        Task AddAsync(T item);

        void Delete(T item);
        void Update(T item);
    }
}
