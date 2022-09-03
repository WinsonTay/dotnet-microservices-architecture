using Shopping.Aggregrator.Models;
using Shopping.Aggregrator.Extensions;

namespace Shopping.Aggregrator.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<OrderResponseModel>> GetOrdersByUserName(string userName)
        {
            var response = await _httpClient.GetAsync($"/api/v1/order/{userName}");
            return await response.ReadContentAs<List<OrderResponseModel>>();
        }
    }
}
