using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIs.Controllers
{
    public class BuggyController : APIControllerBase
    {
        private readonly StoreDbContext _dbcontext;

        public BuggyController(StoreDbContext dbcontext) 
        {
            _dbcontext = dbcontext;
        }
        //NotFoundError
        [HttpGet("NotFound")]
        public ActionResult GetNotFound()
        {
            var product = _dbcontext.Products.Find(100);
            if(product is null)return NotFound(new ApiResponse(404));
            return Ok(product);
        }
        //ServerError
        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var Product = _dbcontext.Products.Find(100);
            var ProductToReturn = Product.ToString();
            return Ok(ProductToReturn);
        }
        //BadRequestError
        [HttpGet("BadRequest")]
        public ActionResult GetBadRequestError()
        {
            return BadRequest(new ApiResponse(400));
        }
        //ValidationError
        [HttpGet("BadRequest/{id}")]
        public ActionResult GetValidationError(int id)
        {
            return Ok();
        }
    }
}
