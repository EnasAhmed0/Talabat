using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repository
{
    public static class SpecificationEvaluator<T> where T : BaseEntity
    {
        //Function To Build Query Dinamically
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery , ISpecifications<T> Spec) 
        {
            var Query = inputQuery;
            if(Spec.Criteria is not null)
            {
                Query = Query.Where(Spec.Criteria);
            }
            if(Spec.OrderBy is not null)
            {
                Query = Query.OrderBy(Spec.OrderBy);
            }
            if(Spec.OrderByDescending is not null)
            { 
                Query = Query.OrderByDescending(Spec.OrderByDescending); 
            }
            if (Spec.IsPaginationEnabled)
            {
                Query = Query.Skip(Spec.Skip).Take(Spec.Take);
            }

            Query = Spec.Includes.Aggregate(Query, (CurrentQuery, IncludeExpression) => CurrentQuery.Include(IncludeExpression));
            return Query;
        }
    }
}
