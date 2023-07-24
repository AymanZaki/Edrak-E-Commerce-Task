using AutoMapper;
using Edrak.Order.Core.Services;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Data.Entities;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models.Enums;
using Moq;
using System.Net;

namespace Edrak.Order.Core.Tests
{
    public class ProductCoreTests
    {
        private readonly ProductCore _productCore;
        private readonly Mock<IProductDal> _productDal;
        private readonly Mock<IMapper> _mapper;

        public ProductCoreTests()
        {
            _productDal = new Mock<IProductDal>();
            _mapper = new Mock<IMapper>();

            _productCore = new ProductCore(_mapper.Object, _productDal.Object);
        }
        [Fact]
        public async Task GetProductByIds_ReturnSuccessfulResultWithProducts()
        {
            // Arrange
            var productIds = new List<int> { 1, 2 };
            var products = new List<Product>
            {
                new Product { Id = 1 },
                new Product { Id = 2 },
            };

            var productDTOs = new List<ProductDTO>
            {
                new ProductDTO { Id = 1 },
                new ProductDTO { Id = 2 },
            };

            _productDal.Setup(mock => mock.GetProductByIds(productIds)).ReturnsAsync(products);
            _mapper.Setup(mock => mock.Map<IEnumerable<ProductDTO>>(products)).Returns(productDTOs);

            // Act
            var result = await _productCore.GetProductByIds(productIds);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal(productDTOs, result.Data);
        }

        [Fact]
        public async Task UpdateProductStock_ValidProductIdsWithRemoveOperationAndSufficientStock_ReturnSuccessfulResult()
        {
            // Arrange
            var orderProductDTO = new List<OrderProductDTO>
            {
                new OrderProductDTO { ProductId = 1, Quantity = 2 },
                new OrderProductDTO { ProductId = 2, Quantity = 3 }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, StockQuantity = 10 },
                new Product { Id = 2, StockQuantity = 15 }
            };

            _productDal.Setup(mock => mock.GetProductByIds(It.IsAny<IEnumerable<int>>())).ReturnsAsync(products);
            _productDal.Setup(mock => mock.UpdateProducts(products)).ReturnsAsync(true);

            // Act
            var result = await _productCore.UpdateProductStock(orderProductDTO, ProductStockOperation.Remove);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Data);
            Assert.Equal(8, products[0].StockQuantity);
            Assert.Equal(12, products[1].StockQuantity);
        }

        [Fact]
        public async Task UpdateProductStock_ValidProductIdsWithAddOperation_ReturnSuccessfulResult()
        {
            // Arrange
            var orderProductDTO = new List<OrderProductDTO>
            {
                new OrderProductDTO { ProductId = 1, Quantity = 3 },
                new OrderProductDTO { ProductId = 2, Quantity = 5 }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, StockQuantity = 10 },
                new Product { Id = 2, StockQuantity = 15 }
            };

            _productDal.Setup(mock => mock.GetProductByIds(It.IsAny<IEnumerable<int>>())).ReturnsAsync(products);
            _productDal.Setup(mock => mock.UpdateProducts(products)).ReturnsAsync(true);

            // Act
            var result = await _productCore.UpdateProductStock(orderProductDTO, ProductStockOperation.Add);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.Data);
            Assert.Equal(13, products[0].StockQuantity);
            Assert.Equal(20, products[1].StockQuantity);
        }

        [Fact]
        public async Task UpdateProductStock_InvalidProductIds_ReturnNotFoundResult()
        {
            // Arrange
            var productIds = new List<int> { 1, 2 };
            var orderProductDTO = new List<OrderProductDTO>
            {
                new OrderProductDTO { ProductId = 1, Quantity = 3 },
                new OrderProductDTO { ProductId = 2, Quantity = 5 }
            };

            var products = new List<Product>
            {
                new Product { Id = 2, StockQuantity = 5 },
                new Product { Id = 3, StockQuantity = 10 }
            };

            var productsFound = new List<Product>
            {
                new Product { Id = 2, StockQuantity = 5 },
            };

            _productDal.Setup(mock => mock.GetProductByIds(productIds)).ReturnsAsync(productsFound);

            // Act
            var result = await _productCore.UpdateProductStock(orderProductDTO, ProductStockOperation.Add);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.False(result.Data);
        }

        [Fact]
        public async Task UpdateProductStock_InsufficientStockForRemoval_ReturnErrorResult()
        {
            // Arrange
            var orderProductDTO = new List<OrderProductDTO>
            {
                new OrderProductDTO { ProductId = 1, Quantity = 5 }
            };

            var products = new List<Product>
            {
                new Product { Id = 1, StockQuantity = 3 }
            };

            _productDal.Setup(mock => mock.GetProductByIds(It.IsAny<IEnumerable<int>>())).ReturnsAsync(products);


            // Act
            var result = await _productCore.UpdateProductStock(orderProductDTO, ProductStockOperation.Remove);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
            Assert.False(result.Data);
        }

    }
}