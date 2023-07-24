using AutoMapper;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Models;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models.Enums;
using Edrak.Order.Models.FilterModel;
using Newtonsoft.Json;
using System.Net;
using OrderStatusEnum = Edrak.Order.Models.Enums.OrderStatus;

namespace Edrak.Order.Core.Services
{
    public class OrderCore : IOrderCore
    {
        private readonly IMapper _mapper; 
        private readonly IOrderDal _orderDal;
        private readonly IProductCore _productCore;
        private readonly ICustomerCore _customerCore;
        public OrderCore(IMapper mapper, IOrderDal orderDal, IProductCore productCore, ICustomerCore customerCore) 
        {
            _mapper = mapper;
            _orderDal = orderDal;
            _productCore = productCore;
            _customerCore = customerCore;
        }

        public async Task<ResultModel<OrderDTO>> CreateOrder(CreateOrderDTO dto)
        {
            var customer = await _customerCore.GetCustomerById(dto.CustomerId);
            if(!customer.IsSuccess)
            {
                return new ResultModel<OrderDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Customer not found",
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            decimal productTotalAmout = 0;
            var productIds = dto.Products.Select(x => x.ProductId).ToList();
            var products = await _productCore.GetProductByIds(productIds);
            if(products is null)
            {
                return new ResultModel<OrderDTO>
                {
                    Data = null,
                    IsSuccess = false,
                    Message = "Products not found",
                    StatusCode = HttpStatusCode.NotFound
                };
            }
            foreach (var product in dto.Products)
            {
                var productDto = products.Data.FirstOrDefault(x => x.Id == product.ProductId);
                
                if(productDto is null)
                {
                    return new ResultModel<OrderDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = $"Product with Id = {product.ProductId} not found",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                bool isAvailable = CheckProductAvailability(product.ProductId, product.Quantity, products.Data);
                if(!isAvailable)
                {
                    return new ResultModel<OrderDTO>
                    {
                        Data = null,
                        IsSuccess = false,
                        Message = $"Product with Id = {product.ProductId} is out of stock",
                        StatusCode = HttpStatusCode.BadRequest
                    };
                }
                productTotalAmout += (product.Quantity * productDto.Price);
            }

            var order = _mapper.Map<Data.Entities.Order>(dto);
            foreach(var item in order.OrderLineItems)
            {
                var metaData = _mapper.Map<OrderProductMetaDataDTO>(products.Data.FirstOrDefault(x => x.Id == item.ProductId));
                item.ProductMetaData = JsonConvert.SerializeObject(metaData);
            }
            var createdOrder = await _orderDal.CreateOrder(order);
            if(createdOrder is null)
            {
                return new ResultModel<OrderDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Data = null,
                    Message = "Failed to create the order",
                };
            }

            await _productCore.UpdateProductStock(dto.Products, ProductStockOperation.Remove);
            var orderDto = _mapper.Map<OrderDTO>(createdOrder);
            return new ResultModel<OrderDTO>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.Created,
                Data = orderDto,
                Message = "Created",
            };
        }
        public async Task<ResultModel<OrderDTO>> GetOrderDetails(int orderId)
        {
            List<string> includes = new List<string> { "OrderLineItems", "Customer" };

            var order = await _orderDal.GetOrderById(orderId, includes);
            if(order is null)
            {
                return new ResultModel<OrderDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Data = null,
                    Message = "not found",
                };
            }
            var orderDto =_mapper.Map<OrderDTO>(order);
            return new ResultModel<OrderDTO>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = orderDto,
                Message = "Succeeded",
            };
        }

        public async Task<ResultModel<bool>> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO dto)
        {
            var order = await _orderDal.GetOrderById(orderId);
            if (order is null)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Data = false,
                    Message = "not found",
                };
            }
            order.StatusId = (int)dto.OrderStatus;
            await _orderDal.UpdateOrder(order);
            return new ResultModel<bool>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.NoContent,
                Data = true,
                Message = "Updated successfully",
            };
        }

        public async Task<ResultModel<bool>> CancleOrder(int orderId)
        {
            var order = await _orderDal.GetOrderById(orderId);
            if (order is null)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Data = false,
                    Message = "not found",
                };
            }
            order.StatusId = (int)OrderStatusEnum.Cancelled;
            await _orderDal.UpdateOrder(order);
            var updateProductDTO = _mapper.Map<IEnumerable<OrderProductDTO>>(order);
            bool isUpdated = (await _productCore.UpdateProductStock(updateProductDTO, ProductStockOperation.Add)).Data;
            if (isUpdated)
            {
                return new ResultModel<bool>
                {
                    IsSuccess = true,
                    StatusCode = HttpStatusCode.NoContent,
                    Data = true,
                    Message = "Updated successfully",
                };
            }
            return new ResultModel<bool>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                Data = false,
                Message = "Failed to cancel order",
            };
        }

        public async Task<ResultModel<IEnumerable<OrderDTO>>> GetOrderByCustomerId(int customerId)
        {
            List<string> includes = new List<string> { "OrderLineItems"};

            var orders = _orderDal.GetOrderByCustomerId(customerId, includes);
            var ordersDto = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            return new ResultModel<IEnumerable<OrderDTO>>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = ordersDto,
                Message = "",
            };
        }
        public async Task<ResultModel<PageModel<OrderDTO>>> GetOrders(OrderFilter filter)
        {
            List<string> includes = new List<string> { "OrderLineItems", "Customer" };
            
            var orders = await _orderDal.GetOrders(filter, includes);
            var ordersCount = await _orderDal.GetOrdersCount(filter);
            var results = new PageModel<OrderDTO>()
            {
                PageNumber = filter.Page,
                PagesCount = (int)Math.Ceiling(ordersCount / (decimal)filter.Limit),
                Limit = filter.Limit,
                TotalCount = ordersCount,
                Results = _mapper.Map<IEnumerable<OrderDTO>>(orders)
            };

            return new ResultModel<PageModel<OrderDTO>>
            {
                Data = results,
                Message = "Succeeded",
                StatusCode = HttpStatusCode.OK
            };

        }

        private bool CheckProductAvailability(int prodcutId, int quantity, IEnumerable<ProductDTO> products)
        {
            if(products != null && products.Any(x => x.Id == prodcutId && x.StockQuantity >= quantity))
            {
                return true;
            }
            return false;
        }

    }
}
