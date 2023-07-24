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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Edrak.Order.Dal.Tests
{
    public class ProductDalTests
    {
        private readonly Mock<IOrdersRepository<Product>> _orderRepository;
        private readonly IProductDal _productDal;
        public ProductDalTests()
        {
            _orderRepository = new Mock<IOrdersRepository<Product>>();
            _productDal = new ProductDal(_orderRepository.Object);
        }

        [Fact]
        public async Task GetProductByIds_ValidProductIds_ReturnProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, IsDeleted = false },
                new Product { Id = 2, IsDeleted = false },
            };

            var productIds = new List<int> { 1, 2 };

            var query = products.AsQueryable();
            _orderRepository.Setup(repo => repo.GetAll()).Returns(query);
            _orderRepository.Setup(repo => repo.GetListAsync(query)).ReturnsAsync(products);

            // Act
            var result = await _productDal.GetProductByIds(productIds);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.False(p.IsDeleted));
        }

        [Fact]
        public async Task GetProductByIds_SomeInvalidProductIds_ReturnValidProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1 },
                new Product { Id = 2 },
            };

            var productIds = new List<int> { 2, 3 };

            var productFound = new List<Product>
            {
                new Product { Id = 2 }
            };

            var query = products.AsQueryable();
            _orderRepository.Setup(repo => repo.GetAll()).Returns(query);
            _orderRepository.Setup(repo => repo.GetListAsync(It.IsAny<IQueryable<Product>>())).ReturnsAsync(productFound);

            // Act
            var result = await _productDal.GetProductByIds(productIds);

            // Assert
            Assert.Equal(1, result.Count());
            Assert.All(result, p => Assert.False(p.IsDeleted));
        }

        [Fact]
        public async Task GetProductByIds_DeletedProductIds_ReturnNonDeletedProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, IsDeleted = false },
                new Product { Id = 2, IsDeleted = false },
                new Product { Id = 3, IsDeleted = true },
            };

            var productIds = new List<int> { 1, 2, 3 };

            var productFound = new List<Product>
            {
                new Product { Id = 1 },
                new Product { Id = 2 }
            };

            var query = products.AsQueryable();
            _orderRepository.Setup(repo => repo.GetAll()).Returns(query);
            _orderRepository.Setup(repo => repo.GetListAsync(It.IsAny<IQueryable<Product>>())).ReturnsAsync(productFound);

            // Act
            var result = await _productDal.GetProductByIds(productIds);

            // Assert
            Assert.Equal(2, result.Count()); 
            Assert.All(result, p => Assert.False(p.IsDeleted));
        }

        [Fact]
        public async Task GetProductByIds_AllInvalidProductIds_Return_EmptyList()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, IsDeleted = false },
                new Product { Id = 2, IsDeleted = false },
                new Product { Id = 3, IsDeleted = false }
            };

            var productIds = new List<int> { 4, 5, 6 };
            _orderRepository.Setup(repo => repo.GetAll()).Returns(products.AsQueryable());
            _orderRepository.Setup(repo => repo.GetListAsync(It.IsAny<IQueryable<Product>>())).ReturnsAsync(new List<Product>());

            // Act
            var result = await _productDal.GetProductByIds(productIds);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateProducts_ValidEntities_Should_Return_True()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", IsDeleted = false },
                new Product { Id = 2, Name = "Product 2", IsDeleted = false },
                new Product { Id = 3, Name = "Product 3", IsDeleted = false }
            };

            
            _orderRepository.Setup(repo => repo.UpdateAsync(It.IsAny<IEnumerable<Product>>())).Returns(Task.CompletedTask);

            
            // Act
            var result = await _productDal.UpdateProducts(products);

            // Assert
            Assert.True(result);
        }
    }
}
