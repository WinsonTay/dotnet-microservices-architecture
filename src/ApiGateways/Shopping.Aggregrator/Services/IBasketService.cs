using Shopping.Aggregrator.Models;
namespace Shopping.Aggregrator.Services
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasket(string userName);
    }
}
