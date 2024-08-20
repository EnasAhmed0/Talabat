using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Services;

namespace Talabat.APIs.Controllers
{

    public class PaymentsController : APIControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        const string endpointSecret = "whsec_c0d719d800efd553a3bf678024c1f18ffe636a2c109e8e45eae5a7a224105a3c";

        public PaymentsController(IPaymentService paymentService, IMapper mapper)
        {
            _paymentService = paymentService;
            _mapper = mapper;
        }
        //Create Or Update PaymentIntent EndPoint
        [ProducesResponseType(typeof(CustomerBasketDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [HttpPost("{BasketId}")]
        [Authorize]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string BasketId)
        {
            var Basket = await _paymentService.CreateOrUpdatePaymentIntent(BasketId);
            if (Basket is null) return BadRequest(new ApiResponse(400, "There Is a Problem With Your Basket"));
            var MappedBasket = _mapper.Map<CustomerBasket, CustomerBasketDto>(Basket);
            return Ok(MappedBasket);

        }

        // Stripe webhook EndPoint
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);
                var PaymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, false);
                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(PaymentIntent.Id, true);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }

        }
    }
}
