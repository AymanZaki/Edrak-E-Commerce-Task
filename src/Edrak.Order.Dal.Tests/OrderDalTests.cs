using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Dal.Repository;
using Edrak.Order.Dal.Services;
using Edrak.Order.Data.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Dal.Tests
{
    public class OrderDalTests
    {
        private readonly Mock<IOrdersRepository<Data.Entities.Order>> _orderRepository;
        private readonly IOrderDal _orderDal;
        public OrderDalTests()
        {
            _orderRepository = new Mock<IOrdersRepository<Data.Entities.Order>>();
            _orderDal = new OrderDal(_orderRepository.Object);
        }

        [Fact]
        public async Task CreateOrder_ValidOrder_ReturnCreatedOrder()
        {
            // Arrange
            var orderToCreate = new Data.Entities.Order { Id = 1, OrderDate = DateTime.Now };

            _orderRepository.Setup(repo => repo.AddAsync(It.IsAny<Data.Entities.Order>()))
                              .ReturnsAsync((Data.Entities.Order order) =>
                              {
                                  order.Id = 1;
                                  return order;
                              });

            // Act
            var createdOrder = await _orderDal.CreateOrder(orderToCreate);

            // Assert
            Assert.NotNull(createdOrder);
            Assert.Equal(1, createdOrder.Id);
            Assert.Equal(orderToCreate.OrderDate, createdOrder.OrderDate);
        }
        [Fact]
        public async Task GetOrderById_ExistingOrderId_ReturnOrder()
        {
            // Arrange
            int orderId = 1;
            var orders = new List<Data.Entities.Order>
            {
                new Data.Entities.Order { Id = 1 },
                new Data.Entities.Order { Id = 2 },
                new Data.Entities.Order { Id = 3 }
            };

            _orderRepository.Setup(repo => repo.GetAll()).Returns(orders.AsQueryable());
            _orderRepository.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<IQueryable<Data.Entities.Order>>()))
                               .ReturnsAsync(new Data.Entities.Order
                               { 
                                   Id = 1
                               });

            // Act
            var result = await _orderDal.GetOrderById(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }
        [Fact]
        public async Task GetOrderById_ExistingOrderIdWithIncludingCustomer_ReturnOrder()
        {
            // Arrange
            int orderId = 1;

            var cutomers = new List<Customer>
            {
                new Customer { Id = 1, Name = "Customer 1" },
                new Customer { Id = 2, Name = "Customer 2" },
            };

            var orders = new List<Data.Entities.Order>
            {
                new Data.Entities.Order { Id = 1, CustomerId = 1, Customer =  cutomers[0] },
                new Data.Entities.Order { Id = 2 , CustomerId = 1, Customer =  cutomers[0] },
                new Data.Entities.Order { Id = 3 , CustomerId = 2, Customer =  cutomers[1] }
            };

            _orderRepository.Setup(repo => repo.GetAll()).Returns(orders.AsQueryable());
            _orderRepository.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<IQueryable<Data.Entities.Order>>()))
                               .ReturnsAsync(new Data.Entities.Order
                               {
                                   Id = 1,
                                   CustomerId = 1,
                                   Customer = cutomers[0]
                               });

            // Act
            var result = await _orderDal.GetOrderById(orderId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Customer);
            Assert.Equal(orderId, result.Id);
            Assert.Equal(1, result.Customer.Id);
        }
    }
}
