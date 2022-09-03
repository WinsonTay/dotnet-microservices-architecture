using Shopping.Aggregrator.Models;
using System.Net.Http;
using Shopping.Aggregrator.Extensions;

namespace Shopping.Aggregrator.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly HttpClient _httpClient;
        public CatalogService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<CatalogModel>> GetCatalog()
        {
            var response = await _httpClient.GetAsync("/api/v1/catalog");
            return await response.ReadContentAs<List<CatalogModel>>();
        }

        public async Task<CatalogModel> GetCatalog(string id)
        {
            var response = await _httpClient.GetAsync($"/api/v1/catalog/{id}");
            return await response.ReadContentAs<CatalogModel>();
        }

        public async Task<IEnumerable<CatalogModel>> GetCatalogByCategory(string category)
        {
            var response = await _httpClient.GetAsync($"/api/v1/catalog/{category}");
            return await response.ReadContentAs<List<CatalogModel>>();
        }
    }
}
