using Edrak.Order.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edrak.Order.Dal.Interfaces
{
    public interface ICustomerDal
    {
        Task<Customer> GetCustomerById(int customerId);
    }
}
