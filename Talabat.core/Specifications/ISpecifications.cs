using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public interface ISpecifications<T> where T:BaseEntity
    {//_dbContext.Products.Where(P=>P.Id==id).Includes(P=>P.ProductType).Includes(P=>P.ProductBrand);

        //Signature for property for Where Condition [Where(P=>P.Id==id)]
        public Expression<Func<T, bool>> Criteria { get; set; }
        //Signature for property for Includes [Includes(P=>P.ProductType).Includes(P=>P.ProductBrand)]
        public List<Expression<Func<T,object>>> Includes { get; set; }

        //Signature For Property For OrderBy Ascending [OrderBy(P=>P.name)]
        public Expression<Func<T,object>> OrderBy { get; set; }
        //Signature For Property For OrderBy Descending [OrderByDesc(P=>P.name)]
        public Expression<Func<T,object>> OrderByDescending { get; set; }

        //Singnature For Property For Skip [Skip(2)]
        public int Skip { get; set; }
        //Singnature For Property For Take [Take(2)]
        public int Take { get; set; }
        //Singnature For Property For IsPaginationEnabled
        public bool IsPaginationEnabled { get; set; }

    }
}
