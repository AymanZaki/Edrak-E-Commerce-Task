using AutoMapper;
using Edrak.Order.Core.Interfaces;
using Edrak.Order.Dal.Interfaces;
using Edrak.Order.Models;
using Edrak.Order.Models.EntityDTOs;
using System.Net;

namespace Edrak.Order.Core.Services
{
    public class CustomerCore : ICustomerCore
    {
        private readonly IMapper _mapper;
        private readonly ICustomerDal _customerDal;
        public CustomerCore(IMapper mapper, ICustomerDal customerDal)
        {
            _mapper = mapper;
            _customerDal = customerDal;
        }

        public async Task<ResultModel<CustomerDTO>> GetCustomerById(int customerId)
        {
            var customer = await _customerDal.GetCustomerById(customerId);
            if(customer is null)
            {
                return new ResultModel<CustomerDTO>
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    Data = null
                };
            }
            var customerDto = _mapper.Map<CustomerDTO>(customer);
            return new ResultModel<CustomerDTO>
            {
                IsSuccess = true,
                StatusCode = HttpStatusCode.OK,
                Data = customerDto
            };
        }
    }
}
