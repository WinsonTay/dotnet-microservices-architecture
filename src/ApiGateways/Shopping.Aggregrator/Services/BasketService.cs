using Shopping.Aggregrator.Models;
using Shopping.Aggregrator.Extensions;
namespace Shopping.Aggregrator.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        public BasketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<BasketModel> GetBasket(string userName)
        {
            var response = await _httpClient.GetAsync($"/api/v1/basket/{userName}");
            return await response.ReadContentAs<BasketModel>();
        }
    }
}
