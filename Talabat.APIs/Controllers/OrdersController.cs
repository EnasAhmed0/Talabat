using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{
    
    public class OrdersController : APIControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
       // private readonly IUnitOfWork _unitOfWork;

        public OrdersController(IOrderService orderService,IMapper mapper/*,IUnitOfWork unitOfWork*/)
        {
            _orderService = orderService;
            _mapper = mapper;
            //_unitOfWork = unitOfWork;
        }



        //Create Order
        [ProducesResponseType(typeof(Order),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var MappedAddress = _mapper.Map<AddressDto, Address>(orderDto.shipToAddress);
            var Order = await _orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);
            if (Order is null) return BadRequest(new ApiResponse(400, "There is a Problem in Your Order"));
            return Ok(Order);
        }


        //Get Orders For User
        [ProducesResponseType(typeof(IReadOnlyList<OrderToReturnDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Orders = await _orderService.GetOrdersForSpecificUserAsync(BuyerEmail);
            if (Orders is null) return NotFound(new ApiResponse(404, "This User Has No Orders"));
            var MappedOrders = _mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(Orders);
            return Ok(MappedOrders);
        }



        //Get Orders By Id For User
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<OrderToReturnDto>> GetOrdersByIdForUser(int id)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Order = await _orderService.GetOrderByIdForSpecificUserAsync(BuyerEmail,id);
            if (Order is null) return NotFound(new ApiResponse(404, $"There is No Order With id = {id} For This User"));
            var mappedOrder = _mapper.Map<Order,OrderToReturnDto>(Order);
            return Ok(mappedOrder);
        }

        [HttpGet("DeliveryMethods")]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            
            //var DeliveryMethods = await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
            //Second Way
            var DeliveryMethods = await _orderService.GetAllDeliveryMethodsAsync();
            return Ok(DeliveryMethods);
        }
    }
}
