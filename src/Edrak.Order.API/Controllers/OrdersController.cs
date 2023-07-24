using AutoMapper;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Dal.Services;
using Edrak.Order.Models;
using Edrak.Order.Models.Contracts;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models.FilterModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Edrak.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IOrderCore _orderCore;
        public OrdersController(IMapper mapper, IOrderCore orderCore) 
        {
            _mapper = mapper;
            _orderCore = orderCore;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDTO request) 
        {
            var orderDto = await _orderCore.CreateOrder(request);

            var orderResponse = _mapper.Map<OrderResponse>(orderDto.Data);
            var response = new ResultModel<OrderResponse>
            {
                IsSuccess = orderDto.IsSuccess,
                Message = orderDto.Message,
                StatusCode = orderDto.StatusCode,
                Data = orderResponse,
            };
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var orderDto = await _orderCore.GetOrderDetails(orderId);

            var orderResponse = _mapper.Map<OrderDetailsResponse>(orderDto.Data);
            var response = new ResultModel<OrderDetailsResponse>
            {
                IsSuccess = orderDto.IsSuccess,
                Message = orderDto.Message,
                StatusCode = orderDto.StatusCode,
                Data = orderResponse,
            };
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDTO request)
        {
            var isUpdated = await _orderCore.UpdateOrderStatus(orderId, request);
            if(isUpdated.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            var response = new ResultModel<bool>
            {
                IsSuccess = isUpdated.IsSuccess,
                Message = isUpdated.Message,
                StatusCode = isUpdated.StatusCode,
                Data = false,
            };
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpPatch("{orderId}/cancel")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var isUpdated = await _orderCore.CancleOrder(orderId);
            if (isUpdated.IsSuccess)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            var response = new ResultModel<bool>
            {
                IsSuccess = isUpdated.IsSuccess,
                Message = isUpdated.Message,
                StatusCode = isUpdated.StatusCode,
                Data = false,
            };
            return StatusCode((int)response.StatusCode, response);
        }

        [HttpGet("")]
        public async Task<IActionResult> GetOrders([FromQuery] OrderFilter request)
        {
            var orders = await _orderCore.GetOrders(request);
            var ordersResponse = _mapper.Map<OrderDetailsResponse>(orders.Data);
            var response = new ResultModel<OrderDetailsResponse>
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
