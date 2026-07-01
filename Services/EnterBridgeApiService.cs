using EnterBridgePOC.Models.Api;
using System.Net.Http.Json;
using System.Text;

namespace EnterBridgePOC.Services
{
    public class EnterBridgeApiService(HttpClient httpClient) : IEnterBridgeApiService
    {
        public async Task<string> GetHealthAsync()
        {
            var response = await httpClient.GetStringAsync("/api/health");
            return response;
        }

        public async Task<PaginatedResponse<ProductDto>> GetProductsAsync(
            int? pageNumber, int? pageSize,
            string? name, string? description, string? sku,
            Category? category, ProductSortBy? sortBy, SortDirection? sortDirection)
        {
            var query = BuildQuery([
                ("PageNumber", pageNumber?.ToString()),
                ("PageSize", pageSize?.ToString()),
                ("name", name),
                ("description", description),
                ("sku", sku),
                ("category", category?.ToString()),
                ("sortBy", sortBy?.ToString()),
                ("sortDirection", sortDirection?.ToString()?.ToLower()),
            ]);

            return await httpClient.GetFromJsonAsync<PaginatedResponse<ProductDto>>($"/api/products{query}")
                ?? new PaginatedResponse<ProductDto>();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id) =>
            await httpClient.GetFromJsonAsync<ProductDto>($"/api/products/{id}");

        public async Task<List<Category>> GetCategoriesAsync() =>
            await httpClient.GetFromJsonAsync<List<Category>>("/api/products/categories")
                ?? [];

        public async Task<PaginatedResponse<PriceDto>> GetPricesAsync(
            int? pageNumber, int? pageSize,
            int? productId, DateTime? startDate, DateTime? endDate,
            double? minAmount, double? maxAmount,
            UnitOfMeasure? unitOfMeasure, PriceSortBy? sortBy, SortDirection? sortDirection)
        {
            var query = BuildQuery([
                ("PageNumber", pageNumber?.ToString()),
                ("PageSize", pageSize?.ToString()),
                ("productId", productId?.ToString()),
                ("startDate", startDate?.ToString("yyyy-MM-dd")),
                ("endDate", endDate?.ToString("yyyy-MM-dd")),
                ("minAmount", minAmount?.ToString()),
                ("maxAmount", maxAmount?.ToString()),
                ("unitOfMeasure", unitOfMeasure?.ToString()),
                ("sortBy", sortBy?.ToString()),
                ("sortDirection", sortDirection?.ToString()?.ToLower()),
            ]);

            return await httpClient.GetFromJsonAsync<PaginatedResponse<PriceDto>>($"/api/prices{query}")
                ?? new PaginatedResponse<PriceDto>();
        }

        public async Task<PriceDto?> GetPriceByIdAsync(int id) =>
            await httpClient.GetFromJsonAsync<PriceDto>($"/api/prices/{id}");

        public async Task<object?> GetStatsAsync() =>
            await httpClient.GetFromJsonAsync<object>("/api/stats");

        public async Task<List<UnitOfMeasure>> GetUnitsOfMeasureAsync() =>
            await httpClient.GetFromJsonAsync<List<UnitOfMeasure>>("/api/unit-of-measures")
                ?? [];

        private static string BuildQuery(IEnumerable<(string Key, string? Value)> parameters)
        {
            var sb = new StringBuilder();
            foreach (var (key, value) in parameters)
            {
                if (value is null) continue;
                sb.Append(sb.Length == 0 ? '?' : '&');
                sb.Append(Uri.EscapeDataString(key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(value));
            }
            return sb.ToString();
        }
    }
}
