using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Repository
{
    public interface IBasketRepository
    {
        //Get Basket
        Task<CustomerBasket?> GetBasketAsync(string BasketId);
        //Create or Update Basket
        Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket Basket);
        //Delete Basket
        Task<bool> DeleteBasketAsync(string BasketId);
    }
}
