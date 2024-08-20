using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Repository;
using Talabat.Core.Services;
using Talabat.Core.Specifications;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration config,IBasketRepository BasketRepo,IUnitOfWork unitOfWork)
        {
            _config = config;
            _basketRepo = BasketRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
        {
            // put the Secret Key In StripeConfiguration
            StripeConfiguration.ApiKey = _config["StripeKeys:SecretKey"];
            // Get Basket
            var Basket = await _basketRepo.GetBasketAsync(BasketId);
            // Check if basket is null
            if (Basket is null) return null;
            // if not null get The DeliveryMethodCost Because the PaymentIntent Depends on the Amount of Basket => subTotal
            // We Go To inject unitOfWorkg 
            var ShippingPrice = 0M;
            if (Basket.DeliveryMethodId.HasValue)
            {
                var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(Basket.DeliveryMethodId.Value);
                ShippingPrice = DeliveryMethod.Cost;
            }
            //Total = SubTotal + shippingPrice
            // We Go To Get Sub Total
            if(Basket.Items.Count > 0)
            {
                foreach (var item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if(item.Price != Product.Price)
                        item.Price = Product.Price;
                }
            }
            var subTotal = Basket.Items.Sum(i => i.Quantity * i.Price);
            // We Go to Create PaymentIntent
            var Service = new PaymentIntentService();
            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(Basket.PaymentIntentId)) // Create
            {
                var Options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(subTotal * 100 + ShippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" },

                };
                paymentIntent = await Service.CreateAsync(Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            else // Update
            {
                var Options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(subTotal * 100 + ShippingPrice * 100)
                };
                paymentIntent = await Service.UpdateAsync(Basket.PaymentIntentId,Options);
                Basket.PaymentIntentId = paymentIntent.Id;
                Basket.ClientSecret = paymentIntent.ClientSecret;
            }
            await _basketRepo.UpdateBasketAsync(Basket);
            return Basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentIntentId, bool flag)
        {
            var spec = new OrderWithPaymentIntentSpec(PaymentIntentId);
            var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if (flag)
            {
                Order.Status = OrderStatus.PaymentReceived;
            }
            else
            {
                Order.Status = OrderStatus.PaymentFailed;
            }
            _unitOfWork.Repository<Order>().Update(Order);
            await _unitOfWork.CompleteAsync();
            return Order;
        }
    }
}
