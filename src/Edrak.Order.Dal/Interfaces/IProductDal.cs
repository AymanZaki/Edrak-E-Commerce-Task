using Edrak.Order.Data.Entities;

namespace Edrak.Order.Dal.Interfaces
{
    public interface IProductDal
    {
        Task<IEnumerable<Product>> GetProductByIds(IEnumerable<int> productIds);
        Task<bool> UpdateProducts(IEnumerable<Product> entities);
    }
}
