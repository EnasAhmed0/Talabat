using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Services
{
    public interface IOrderService
    {
        // Signature for Function => CreateOrderAsync
        Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodid, Address ShippingAddress);

        // Signature for Dunction => GetOrdersForSpecificUserAsync

        Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string BuyerEmail);

        // Signature For Function => GetOrderByIdForSpecificUserAsync

        Task<Order> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int OrderId);

        // Signature For function => Get All Delivery Methods
        Task<IReadOnlyList<DeliveryMethod>> GetAllDeliveryMethodsAsync();
    }
}
