using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;

namespace Talabat.Core.Services
{
    public interface IPaymentService
    {
        // Signature For Create Or Update PaymentIntent
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId);

        // Signature For Function Of Stripe Webhook
        Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentIntentId, bool flag);
    }
}
