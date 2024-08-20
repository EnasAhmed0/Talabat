using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndTypeSpecifications:BaseSpecifications<Product>
    {
        // CTOR is Used to Get All Products
        public ProductWithBrandAndTypeSpecifications(ProductSpecParams param)
            :base(P=>
            (string.IsNullOrEmpty(param.Search) || P.Name.ToLower().Contains(param.Search))
            &&
            (!param.BrandId.HasValue || P.ProductBrandId ==  param.BrandId)
            &&
            (!param.TypeId.HasValue || P.ProductTypeId == param.TypeId))
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);
            if (!string.IsNullOrEmpty(param.Sort))
            {
                switch(param.Sort)
                {
                    case "PriceAsc":
                        OrederBy(P => P.Price);
                        break;
                    case "PriceDesc":
                        OrederByDesc(P => P.Price);
                        break;
                    default:
                        OrederBy(P => P.Name);
                        break;
                }
            }
            //For Example to put the correct answer
            //Products =100
            //PageSize = 10
            //PageIndex = 5 
            ApplyPagination(param.PageSize *(param.PageIndex - 1),param.PageSize);
        } 
        public ProductWithBrandAndTypeSpecifications(int id):base(P=>P.Id == id)
        {
            Includes.Add(P => P.ProductType);
            Includes.Add(P => P.ProductBrand);
        }
    }
}
