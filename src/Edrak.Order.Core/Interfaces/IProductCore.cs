using Edrak.Order.Models;
using Edrak.Order.Models.DTOs;
using Edrak.Order.Models.EntityDTOs;
using Edrak.Order.Models.Enums;

namespace Edrak.Order.Core.Interfaces
{
    public interface IProductCore
    {
        Task<ResultModel<IEnumerable<ProductDTO>>> GetProductByIds(IEnumerable<int> productIds);
        Task<ResultModel<bool>> UpdateProductStock(IEnumerable<OrderProductDTO> orderProductDTO, ProductStockOperation operation);
    }
}