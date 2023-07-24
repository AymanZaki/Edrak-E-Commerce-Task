using AutoMapper;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Core.Services;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Data.Entities;
using Edrak.Order.Models.EntityDTOs;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Core.Tests
{
    public class CustomerCoreTests
    {
        private readonly CustomerCore _customerCore;
        private readonly Mock<ICustomerDal> _customerDal;
        private readonly Mock<IMapper> _mapper;

        public CustomerCoreTests()
        {
            _customerDal = new Mock<ICustomerDal>();
            _mapper = new Mock<IMapper>();

            _customerCore = new CustomerCore(_mapper.Object, _customerDal.Object);
        }

        [Fact]
        public async Task GetCustomerById_ExistingCustomer_ReturnsResultModelWithCustomerDTO()
        {
            // Arrange
            var customerId = 1;
            var expectedCustomer = new Customer { Id = customerId };
            
            _mapper.Setup(mapper => mapper.Map<CustomerDTO>(expectedCustomer))
                .Returns(new CustomerDTO { Id = customerId });
            _customerDal.Setup(f => f.GetCustomerById(customerId)).ReturnsAsync(expectedCustomer);

            // Act
            var result = await _customerCore.GetCustomerById(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Equal(expectedCustomer.Id, result.Data.Id);
        }

        [Fact]
        public async Task GetCustomerById_NonExistingCustomer_ReturnsResultModelWithNullData()
        {
            // Arrange
            var customerId = 10; // Assuming 10 does not exist in the mock database
            Customer expectedCustomer = null;

            _customerDal.Setup(f => f.GetCustomerById(customerId)).ReturnsAsync(expectedCustomer);

            // Act
            var result = await _customerCore.GetCustomerById(customerId);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(result.Data);
        }
    }
}
