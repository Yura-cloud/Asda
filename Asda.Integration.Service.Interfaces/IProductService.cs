using Asda.Integration.Domain.Models.Products;

namespace Asda.Integration.Service.Intefaces
{
    public interface IProductService
    {
        ProductInventoryUpdateResponse SendInventoryUpdate(ProductInventoryUpdateRequest request);
    }
}