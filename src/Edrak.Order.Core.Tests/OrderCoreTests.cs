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

namespace Edrak.Order.Core.Tests
{
    public class OrderCoreTests
    {

        private readonly OrderCore _orderCore;
        private readonly Mock<IOrderDal> _orderDal;
        private readonly Mock<IProductCore> _productCore;
        private readonly Mock<IMapper> _mapper;

        public OrderCoreTests()
        {
            _orderDal = new Mock<IOrderDal>();
            _productCore = new Mock<IProductCore>();
            _mapper = new Mock<IMapper>();

            _orderDal.Setup(x => x.CreateOrder(It.IsAny<OrderEntity>())).Returns<OrderEntity>(x => Task.FromResult(x));

        }

    }
}
