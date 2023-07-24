using AutoMapper;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Models;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models.Enums;
using System.Net;

namespace Edrak.Order.Core.Services
{
    public class ProductCore : IProductCore
    {
        private readonly IMapper _mapper;
        private readonly IProductDal _productDal;
        public ProductCore(IMapper mapper, IProductDal productDal)
        {
            _mapper = mapper;
            _productDal = productDal;
        }

        public async Task<ResultModel<IEnumerable<ProductDTO>>> GetProductByIds(IEnumerable<int> productIds)
        {
            var products = await _productDal.GetProductByIds(productIds);
            var productsDto = _mapper.Map<IEnumerable<ProductDTO>>(products);
            return new ResultModel<IEnumerable<ProductDTO>>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = productsDto
            };
        }

        public async Task<ResultModel<bool>> UpdateProductStock(IEnumerable<OrderProductDTO> orderProductDTO, ProductStockOperation operation)
        {
            var productIds = orderProductDTO.Select(x => x.ProductId).ToList();
            var products = await _productDal.GetProductByIds(productIds);

            bool isValid = products.Count(p => productIds.Contains(p.Id)) == productIds.Count;
            if (!isValid)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Data = false
                };
            }
            foreach (var product in products)
            {
                int orderQuantity = orderProductDTO.FirstOrDefault(x => x.ProductId == product.Id).Quantity;

                if (operation == ProductStockOperation.Remove)
                {
                    if (product.StockQuantity - orderQuantity >= 0)
                    {
                        product.StockQuantity -= orderQuantity;
                    }
                    else
                    {
                        return new ResultModel<bool>
                        {
                            IsSuccess = false,
                            StatusCode = HttpStatusCode.InternalServerError,
                            Data = false,
                            Message = "StockQuantity can't be less than Zero"
                        };
                    }
                }
                else
                {
                    product.StockQuantity += orderQuantity;
                }
            }
            bool isUpdated = await _productDal.UpdateProducts(products);
            if(!isUpdated)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = false
                };
            }
            return new ResultModel<bool>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = true
            };
        }
    }
}
