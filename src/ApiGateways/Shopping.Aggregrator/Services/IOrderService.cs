using Shopping.Aggregrator.Models;
namespace Shopping.Aggregrator.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName);
    }
}
