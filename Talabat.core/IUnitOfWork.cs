using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repository;

namespace Talabat.Core
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        //Signature For Fuction to Generate Object For GenericRepository For any Class Inherit BaseEntity
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        //Signature For Complete => SaveChangesAsync
        Task<int> CompleteAsync();
    }
}
