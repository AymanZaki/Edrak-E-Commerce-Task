using AutoMapper;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Models.Contracts;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Edrak.Order.Dal.Services;

namespace Edrak.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderCore _orderCore;
        public CustomersController(IMapper mapper, IOrderCore orderCore) 
        {
            _mapper = mapper;
            _orderCore = orderCore;
        }

        [HttpGet("{customerId}/orders")]
        public async Task<IActionResult> GetCustomerOrders(int customerId)
        {
            var orders = await _orderCore.GetOrderByCustomerId(customerId);
            var ordersResponse = _mapper.Map<IEnumerable<OrderResponse>>(orders.Data);
            var response = new ResultModel<IEnumerable<OrderResponse>>
            {
                IsSuccess = orders.IsSuccess,
                Message = orders.Message,
                StatusCode = orders.StatusCode,
                Data = ordersResponse,
            };
            return StatusCode((int)response.StatusCode, response);

        }
    }
}
