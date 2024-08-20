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
using Talabat.Core.Specifications.Order_Spec;

namespace Talabat.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        //private readonly IGenericRepository<Product> _productRepo;
        //private readonly IGenericRepository<DeliveryMethod> _deliveryMethod;
        //private readonly IGenericRepository<Order> _orderRepo;

        public OrderService(IBasketRepository BasketRepo,IUnitOfWork unitOfWork,IPaymentService paymentService
                            /*IGenericRepository<Product> ProductRepo,
                             IGenericRepository<DeliveryMethod> DeliveryMethod,
                              IGenericRepository<Order> OrderRepo*/)
        {
            _basketRepo = BasketRepo;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
            //_productRepo = ProductRepo;
            //_deliveryMethod = DeliveryMethod;
            //_orderRepo = OrderRepo;
        }
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethodid, Address ShippingAddress)
        {
            //1. Get Basket From BasketRepo
            var Basket = await _basketRepo.GetBasketAsync(BasketId);
            //2. Get Selected Items At Basket From ProductRepo
            var OrderItems = new List<OrderItem>();
            if(Basket?.Items.Count > 0)
            {
                foreach(var item in Basket.Items)
                {
                    var Product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    var ProductOrderedItem = new ProductItemOrdered(Product.Id,Product.Name,Product.PictureUrl);
                    var OrderItem = new OrderItem(ProductOrderedItem, item.Quantity, Product.Price);

                    OrderItems.Add(OrderItem);
                }
            }
            //3. Calculate SubTotal = Price * Quantity
            var SubTotal = OrderItems.Sum(item => item.Price * item.Quantity);
            //4. Get Delivery Method From DeliveryMethodRepo
            var DeliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethodid);
            //5. Create Order

            var spec = new OrderWithPaymentIntentSpec(Basket.PaymentIntentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(BasketId);
            }
            var Order = new Order(BuyerEmail,ShippingAddress,DeliveryMethod,OrderItems,SubTotal,Basket.PaymentIntentId);
            //6. Add Order Locally
            await _unitOfWork.Repository<Order>().AddAsync(Order);
            //7. Save Order to Database [ToDo]
            var Result = await _unitOfWork.CompleteAsync();
            if (Result <= 0) return null;
            return Order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetAllDeliveryMethodsAsync()
        {
            var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            return DeliveryMethods;
        }

        public async Task<Order> GetOrderByIdForSpecificUserAsync(string BuyerEmail, int OrderId)
        {
            var Spec = new OrderSpecifications(BuyerEmail, OrderId);
            var Order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(Spec);
            return Order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string BuyerEmail)
        {

            var Spec = new OrderSpecifications(BuyerEmail);
            var Orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(Spec);
            return Orders;
        }
    }
}
