using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public bool IsPaginationEnabled { get; set; }

        //Get All
        public BaseSpecifications()
        {
            //Includes = new List<Expression<Func<T, object>>>();
        }
        //Get By Id
        public BaseSpecifications(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
            //Includes = new List<Expression<Func<T, object>>>();
        }

        //Function to take The Expression for OrderBy and initialize it in its property
        public void OrederBy(Expression<Func<T,object>> OrderByExpression)
        {
            OrderBy = OrderByExpression;
        //Function to take The Expression for OrderBy and initialize it in its property
        } public void OrederByDesc(Expression<Func<T,object>> OrderByDescExpression)
        {
            OrderByDescending = OrderByDescExpression;
        }
        //Function To Take the Two Values of Skip and Take
        public void ApplyPagination(int skip , int take)
        {
            IsPaginationEnabled = true;
            Skip= skip;
            Take = take;
        }
    }
}
