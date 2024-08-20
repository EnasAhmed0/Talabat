using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repository;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{

    public class ProductsController : APIControllerBase
    {
        //private readonly IGenericRepository<Product> _productRepo;
        //private readonly IGenericRepository<ProductType> _typeRepo;
        //private readonly IGenericRepository<ProductBrand> _brandRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductsController(IMapper mapper, IUnitOfWork unitOfWork/*IGenericRepository<Product> ProductRepo , 
                                  IGenericRepository<ProductType> TypeRepo,IGenericRepository<ProductBrand> BrandRepo ,*/ )
        {
            //_productRepo = ProductRepo;
            //_typeRepo = TypeRepo;
            //_brandRepo = BrandRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //Get All Products
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<Pagination<ProductToReturnDTO>>> GetAllProducts([FromQuery]ProductSpecParams param)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(param);
            //var products =await _productRepo.GetAllAsync();
            var products =await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);
            var MappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDTO>>(products);
            ///var ReturnedObject = new Pagination<ProductToReturnDTO>()
            ///{
            ///    PageIndex = param.PageIndex,
            ///    PageSize = param.PageSize,
            ///    Data = MappedProducts
            ///};
            ///return Ok(ReturnedObject);
            var CountSpec = new ProductWithFilterationForCountSpecAsync(param);
            var Count = await _unitOfWork.Repository<Product>().GetCountWithSpecAsync(CountSpec);

            return Ok(new Pagination<ProductToReturnDTO>(param.PageIndex,param.PageSize,MappedProducts,Count));
            
        }

        //Get By Id 

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductToReturnDTO),200)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductToReturnDTO>> GetProduct(int id)
        {
            var spec = new ProductWithBrandAndTypeSpecifications(id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            if (product is null) return NotFound(new ApiResponse(404));
            var MappedProduct = _mapper.Map<Product,ProductToReturnDTO>(product);

            //var product = await _productRepo.GetByIdAsync(id);
            return Ok(MappedProduct);
        }

        // Get All Types
        //BaseUrl/api/Product/Types
        [HttpGet("Types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetAllProductTypes()
        {
            var Types = await _unitOfWork.Repository<ProductType>().GetAllAsync();
            return Ok(Types);
        }
        
        // Get All Brands
        //BaseUrl/api/Product/Brands
        [HttpGet("Brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetAllProductBrands()
        {
            var Brands = await _unitOfWork.Repository<ProductBrand>().GetAllAsync();
            return Ok(Brands);
        }

    }
}
