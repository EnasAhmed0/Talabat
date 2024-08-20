using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repository;

namespace Talabat.APIs.Controllers
{
    
    public class BasketController : APIControllerBase
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IMapper _mapper;

        public BasketController(IBasketRepository basketRepo,IMapper mapper) 
        {
            _basketRepo = basketRepo;
            _mapper = mapper;
        }

        //Get or ReCreate Basket
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket(string BasketId)
        {
            var Basket = await _basketRepo.GetBasketAsync(BasketId);
            return Basket is null ? new CustomerBasket(BasketId) : Basket;
        }

        // Update or Create New Basket
        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto Basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(Basket);
            var CreatedOrUpdate = await _basketRepo.UpdateBasketAsync(MappedBasket);
            if (CreatedOrUpdate is null) return BadRequest(new ApiResponse(400));
            return Ok(CreatedOrUpdate);
        }

        // Delete Basket
        [HttpDelete]
        public async Task<ActionResult<bool>> DeleteBasket(string BasketId)
        {
            return await _basketRepo.DeleteBasketAsync(BasketId);
        }
    }
}
