using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Dal.Repository;
using Edrak.Order.Dal.Services;
using Edrak.Order.Data.Entities;
using Edrak.Order.Models.EntityDTOs;
using Moq;

namespace Edrak.Order.Dal.Tests
{
    public class CustomerDalTests
    {
        private readonly Mock<IOrdersRepository<Customer>> _orderRepository;
        private readonly CustomerDal _customerDal;
        public CustomerDalTests()
        {
            _orderRepository = new Mock<IOrdersRepository<Customer>>();
            _customerDal = new CustomerDal(_orderRepository.Object);
        }
        [Fact]
        public async Task GetCustomerById_ExistingCustomerId_Should_Return_Customer()
        {
            // Arrange
            int customerId = 1;
            var customers = new List<Customer>
            {
                new Customer { Id = 1 },
                new Customer { Id = 2 },
            };
            var query = customers.AsQueryable();
            _orderRepository.Setup(repo => repo.GetAll()).Returns(query);
            _orderRepository.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<IQueryable<Customer>>()))
                               .ReturnsAsync(new Customer { Id = 1 });

            // Act
            var result = await _customerDal.GetCustomerById(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(customers[0].Id, result.Id);
        }

        [Fact]
        public async Task GetCustomerById_NonExistingCustomerId_ReturnNull()
        {
            // Arrange
            int customerId = 5;
            var customers = new List<Customer>
            {
                new Customer { Id = 1 },
                new Customer { Id = 2 }
            };

            _orderRepository.Setup(repo => repo.GetAll()).Returns(customers.AsQueryable());
            _orderRepository.Setup(repo => repo.GetFirstOrDefaultAsync(It.IsAny<IQueryable<Customer>>()))
                               .ReturnsAsync((Customer?)null);

            // Act
            var result = await _customerDal.GetCustomerById(customerId);

            // Assert
            Assert.Null(result);
        }
    }
}
