using AutoMapper;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Core.Services;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Models;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models.Enums;
using Moq;
using System.Net;
using OrderStatusEnum = Edrak.Order.Models.Enums.OrderStatus;
using OrderEntity = Edrak.Order.Data.Entities.Order;
using Edrak.Order.Data.Entities;
using Castle.Core.Resource;
using Edrak.Order.Models.FilterModel;

namespace Edrak.Order.Core.Tests
{
    public class OrderCoreTests
    {

        private readonly OrderCore _orderCore;
        private readonly Mock<IOrderDal> _orderDal;
        private readonly Mock<IProductCore> _productCore;
        private readonly Mock<ICustomerCore> _customerCore;
        private readonly Mock<IMapper> _mapper;

        public OrderCoreTests()
        {
            _orderDal = new Mock<IOrderDal>();
            _productCore = new Mock<IProductCore>();
            _customerCore = new Mock<ICustomerCore>();

            _mapper = new Mock<IMapper>();

            _orderCore = new OrderCore(_mapper.Object, _orderDal.Object, _productCore.Object, _customerCore.Object);

            _orderDal.Setup(x => x.CreateOrder(It.IsAny<OrderEntity>())).Returns<OrderEntity>(x => Task.FromResult(x));

        }
        [Fact]
        public async Task CreateOrder_CustomerNotFound_ReturnFailureResult()
        {
            // Arrange
            var customerId = 10;
            var dto = new CreateOrderDTO
            {
                CustomerId = customerId,
            };

            _customerCore.Setup(mock => mock.GetCustomerById(customerId))
                             .ReturnsAsync(new ResultModel<CustomerDTO>
                             {
                                 IsSuccess = false,
                                 StatusCode = HttpStatusCode.NotFound
                             });

            // Act
            var result = await _orderCore.CreateOrder(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateOrder_ProductNotFound_ReturnFailureResult()
        {
            // Arrange
            var dto = new CreateOrderDTO
            {
                CustomerId = 1,
                Products = new List<OrderProductDTO>
                {
                    new OrderProductDTO { ProductId = 1, Quantity = 3 },
                }
            };

            _customerCore.Setup(mock => mock.GetCustomerById(It.IsAny<int>()))
                             .ReturnsAsync(new ResultModel<CustomerDTO>
                             {
                                 IsSuccess = true,
                             });
            _productCore.Setup(mock => mock.GetProductByIds(new List<int> { 1, 2 }))
                             .ReturnsAsync(new ResultModel<IEnumerable<ProductDTO>>
                             {
                                 IsSuccess = true,
                                 Data = new List<ProductDTO>
                                 {
                                     new ProductDTO { Id = 1 }
                                 }
                             });
            // Act
            var result = await _orderCore.CreateOrder(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task CreateOrder_ProductsFoundButNoStock_ReturnFailureResult()
        {
            // Arrange
            var dto = new CreateOrderDTO
            {
                CustomerId = 1,
                Products = new List<OrderProductDTO>
                {
                    new OrderProductDTO { ProductId = 1, Quantity = 3 },
                    new OrderProductDTO { ProductId = 2, Quantity = 3 },
                }
            };

            var productsDto = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, StockQuantity = 2 },
                new ProductDTO { Id = 2, StockQuantity = 5 },
            };

            _customerCore.Setup(mock => mock.GetCustomerById(It.IsAny<int>()))
                             .ReturnsAsync(new ResultModel<CustomerDTO>
                             {
                                 IsSuccess = true,
                             });
            _productCore.Setup(mock => mock.GetProductByIds(new List<int> { 1, 2 }))
                             .ReturnsAsync(new ResultModel<IEnumerable<ProductDTO>>
                             {
                                 IsSuccess = true,
                                 Data = productsDto
                             });

            // Act
            var result = await _orderCore.CreateOrder(dto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Null(result.Data);
        }
        [Fact]
        public async Task CreateOrder_ReturnTheCreatedOrder()
        {
            // Arrange
            var dto = new CreateOrderDTO
            {
                CustomerId = 1,
                Products = new List<OrderProductDTO>
                {
                    new OrderProductDTO { ProductId = 1, Quantity = 3 },
                    new OrderProductDTO { ProductId = 2, Quantity = 2 },
                }
            };

            var productsDto = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, StockQuantity = 5, Price = 2 },
                new ProductDTO { Id = 2, StockQuantity = 5, Price = 10 },
            };

            _customerCore.Setup(mock => mock.GetCustomerById(It.IsAny<int>()))
                             .ReturnsAsync(new ResultModel<CustomerDTO>
                             {
                                 IsSuccess = true,
                             });
            _productCore.Setup(mock => mock.GetProductByIds(new List<int> { 1, 2 }))
                             .ReturnsAsync(new ResultModel<IEnumerable<ProductDTO>>
                             {
                                 IsSuccess = true,
                                 Data = productsDto
                             });

            var createdOrder = new OrderEntity
            {
                Id = 1,
                CustomerId = 1,
                OrderLineItems = new List<OrderLineItem>
                {
                    new OrderLineItem { ProductId = 1, Quantity = 3, },
                    new OrderLineItem { ProductId = 2, Quantity = 2, }
                }
            };
            var expectedCreatedOrderDTO = new OrderDTO
            {
                Id = 1,
                Status = OrderStatusEnum.Pending,
                TotalAmount = dto.Products.Sum(x => (x.Quantity * productsDto.FirstOrDefault(p => p.Id == x.ProductId).Price)),
                OrderLineItems = new List<OrderLineItemDTO>
                {
                    new OrderLineItemDTO { ProductId = 1, Quantity = 3, },
                    new OrderLineItemDTO { ProductId = 2, Quantity = 2, }
                }
            };
            _mapper.Setup(mapper => mapper.Map<OrderEntity>(dto))
                .Returns(createdOrder);


            _mapper.Setup(mapper => mapper.Map<OrderProductMetaDataDTO>(It.IsAny<ProductDTO>()))
                .Returns(It.IsAny<OrderProductMetaDataDTO>());

            _mapper.Setup(mapper => mapper.Map<OrderDTO>(createdOrder))
                .Returns(expectedCreatedOrderDTO);

            // Act
            var result = await _orderCore.CreateOrder(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.Created, result.StatusCode);
            Assert.Equal(expectedCreatedOrderDTO, result.Data);
            Assert.Equal(26, result.Data.TotalAmount);
        }

        [Fact]
        public async Task GetOrderDetails_OrderNotFound_ReturnsFailureResult()
        {
            // Arrange
            var orderId = 123;

            _orderDal.Setup(mock => mock.GetOrderById(orderId, It.IsAny<List<string>>()))
                       .ReturnsAsync((OrderEntity)null);

            // Act
            var result = await _orderCore.GetOrderDetails(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task GetOrderDetails_OrderFound_ReturnSuccessResult()
        {
            // Arrange
            int orderId = 1;

            var mockOrder = new OrderEntity
            {
                Id = orderId,
            };

            _orderDal.Setup(mock => mock.GetOrderById(orderId, It.IsAny<List<string>>()))
                       .ReturnsAsync(mockOrder);

            _mapper.Setup(mock => mock.Map<OrderDTO>(mockOrder))
                      .Returns(new OrderDTO
                      {
                          Id = orderId,
                      });

            // Act
            var result = await _orderCore.GetOrderDetails(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(orderId, result.Data.Id);
        }

        [Fact]
        public async Task UpdateOrderStatus_OrderExistsStatusUpdatedSuccessfully_ReturnsSuccessResult()
        {
            // Arrange
            int orderId = 1;
            var updateStatusDto = new UpdateOrderStatusDTO
            {
                OrderStatus = OrderStatusEnum.Delevied
            };

            // Create a mock order for testing purposes
            var mockOrder = new OrderEntity
            {
                Id = orderId,
            };

            _orderDal.Setup(mock => mock.GetOrderById(orderId, It.IsAny<List<string>>()))
                       .ReturnsAsync(mockOrder);

            // Act
            var result = await _orderCore.UpdateOrderStatus(orderId, updateStatusDto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task UpdateOrderStatus_OrderNotFound_ReturnFailureResult()
        {
            // Arrange
            var orderId = 111;
            var updateStatusDto = new UpdateOrderStatusDTO
            {
                OrderStatus = OrderStatusEnum.Shipped
            };

            _orderDal.Setup(mock => mock.GetOrderById(orderId, It.IsAny<List<string>>()))
                       .ReturnsAsync((OrderEntity)null);

            // Act
            var result = await _orderCore.UpdateOrderStatus(orderId, updateStatusDto);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Data);
        }

        [Fact]
        public async Task CancelOrder_OrderExists_CancellationAndProductStockUpdateSuccessful_ReturnSuccessResult()
        {
            // Arrange
            int orderId = 1;

            var mockOrder = new OrderEntity
            {
                Id = orderId,
            };

            _orderDal.Setup(mock => mock.GetOrderById(orderId, It.IsAny<List<string>>()))
                       .ReturnsAsync(mockOrder);

            _productCore.Setup(mock => mock.UpdateProductStock(It.IsAny<IEnumerable<OrderProductDTO>>(), ProductStockOperation.Add))
                           .ReturnsAsync(new ResultModel<bool> { IsSuccess = true, Data = true });

            // Act
            var result = await _orderCore.CancelOrder(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task CancelOrder_OrderNotFound_ReturnFailureResult()
        {
            // Arrange
            var orderId = 123;

            _orderDal.Setup(mock => mock.GetOrderById(orderId, It.IsAny<List<string>>()))
                       .ReturnsAsync((OrderEntity)null);

            // Act
            var result = await _orderCore.CancelOrder(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Data);
        }
        [Fact]
        public async Task CancelOrder_ProductStockUpdateFails_ReturnsFailureResult()
        {
            // Arrange
            var orderId = 123;

            var mockOrder = new OrderEntity
            {
                Id = orderId
            };

            _orderDal.Setup(mock => mock.GetOrderById(orderId, It.IsAny<List<string>>()))
                       .ReturnsAsync(mockOrder);

            _productCore.Setup(mock => mock.UpdateProductStock(It.IsAny<IEnumerable<OrderProductDTO>>(), ProductStockOperation.Add))
                           .ReturnsAsync(new ResultModel<bool> { IsSuccess = false, Data = false });

            // Act
            var result = await _orderCore.CancelOrder(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Data);
        }

        [Fact]
        public async Task GetOrders_ValidFilter_ReturnsSuccessResultWithPageModel()
        {
            // Arrange
            var filter = new OrderFilter
            {
                Page = 1,
                Limit = 2,
            };

            var orders = new List<OrderEntity>
            {
                new OrderEntity { Id = 1, },
                new OrderEntity { Id = 2, },
                new OrderEntity { Id = 3, },
            };

            var filterdOrders = new List<OrderEntity>
            {
                new OrderEntity { Id = 1, },
                new OrderEntity { Id = 2, },
            };

            _orderDal.Setup(mock => mock.GetOrders(filter, It.IsAny<List<string>>()))
                       .ReturnsAsync(filterdOrders);

            _orderDal.Setup(mock => mock.GetOrdersCount(filter))
                       .ReturnsAsync(orders.Count);

            _mapper.Setup(mock => mock.Map<IEnumerable<OrderDTO>>(filterdOrders))
                      .Returns(new List<OrderDTO>
                      {
                          new OrderDTO { Id = 1, },
                          new OrderDTO { Id = 2, },
                      });

            // Act
            var result = await _orderCore.GetOrders(filter);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(filter.Page, result.Data.PageNumber);
            Assert.Equal(2, result.Data.PagesCount);
            Assert.Equal(filter.Limit, result.Data.Limit);
            Assert.Equal(orders.Count, result.Data.TotalCount);
            Assert.Equal(filterdOrders.Count, result.Data.Results.Count());
        }

        [Fact]
        public async Task GetOrders_ValidFilterWithNoOrdersFound_ReturnSuccessResultWithEmptyPageModel()
        {
            // Arrange
            var filter = new OrderFilter
            {
                Page = 1,
                Limit = 10,
            };

            _orderDal.Setup(mock => mock.GetOrders(filter, It.IsAny<List<string>>()))
                       .ReturnsAsync(new List<OrderEntity>());

            _orderDal.Setup(mock => mock.GetOrdersCount(filter))
                       .ReturnsAsync(0);

            // Act
            var result = await _orderCore.GetOrders(filter);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal("Succeeded", result.Message);
            Assert.Equal(1, result.Data.PageNumber);
            Assert.Equal(0, result.Data.PagesCount);
            Assert.Equal(10, result.Data.Limit);
            Assert.Equal(0, result.Data.TotalCount);
            Assert.Empty(result.Data.Results);
        }
    }
}
