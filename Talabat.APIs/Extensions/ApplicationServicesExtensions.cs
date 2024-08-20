using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repository;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection Services)
        {
            
            Services.AddScoped(typeof(IPaymentService),typeof(PaymentService));
            Services.AddScoped(typeof(IOrderService),typeof(OrderService));
            Services.AddScoped(typeof(IUnitOfWork),typeof(UnitOfWork));
            Services.AddScoped(typeof(IBasketRepository),typeof(BasketRepository));
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            //builder.Services.AddAutoMapper(M=>M.AddProfile(new MappingProfilles()));
            Services.AddAutoMapper(typeof(MappingProfilles));

            Services.Configure<ApiBehaviorOptions>(Options =>
            {
                Options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(P => P.Value.Errors.Count() > 0)
                                                         .SelectMany(P => P.Value.Errors)
                                                         .Select(E => E.ErrorMessage)
                                                         .ToList();

                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });
            return Services;
        }
    }
}
