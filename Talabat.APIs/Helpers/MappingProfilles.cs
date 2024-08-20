using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Aggregate;
using OrderAddress = Talabat.Core.Entities.Order_Aggregate.Address;
using IdentityAddress = Talabat.Core.Entities.Identity.Address;

namespace Talabat.APIs.Helpers
{
    public class MappingProfilles : Profile
    {
        public MappingProfilles()
        {
            CreateMap<Product, ProductToReturnDTO>()
                     .ForMember(d=>d.ProductBrand,O=>O.MapFrom(S=>S.ProductBrand.Name))
                     .ForMember(d=>d.ProductType,O=>O.MapFrom(S=>S.ProductType.Name))
                     .ForMember(d=>d.PictureUrl,O=>O.MapFrom<ProductPictureUrlResolver>());
            // Go to Up Page to Show The Alias Name For nameSpaces Which i use it here
            CreateMap<IdentityAddress, AddressDto>().ReverseMap();
            CreateMap<AddressDto, OrderAddress>();

            CreateMap<CustomerBasketDto,CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto,BasketItem>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                     .ForMember(d=>d.DeliveryMethod,O=>O.MapFrom(S=>S.DeliveryMethod.ShortName))
                     .ForMember(d=>d.DeliveryMethodCost,O=>O.MapFrom(S=>S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                    .ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
                    .ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
                    .ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
                    .ForMember(d => d.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());
        
        }
    }
}
