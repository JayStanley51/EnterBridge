using EnterBridgePOC.Models.Api;

namespace EnterBridgePOC.Services
{
    public interface IEnterBridgeApiService
    {
        Task<string> GetHealthAsync();

        Task<PaginatedResponse<ProductDto>> GetProductsAsync(
            int? pageNumber, int? pageSize,
            string? name, string? description, string? sku,
            Category? category, ProductSortBy? sortBy, SortDirection? sortDirection);

        Task<ProductDto?> GetProductByIdAsync(int id);

        Task<List<Category>> GetCategoriesAsync();

        Task<PaginatedResponse<PriceDto>> GetPricesAsync(
            int? pageNumber, int? pageSize,
            int? productId, DateTime? startDate, DateTime? endDate,
            double? minAmount, double? maxAmount,
            UnitOfMeasure? unitOfMeasure, PriceSortBy? sortBy, SortDirection? sortDirection);

        Task<PriceDto?> GetPriceByIdAsync(int id);

        Task<object?> GetStatsAsync();

        Task<List<UnitOfMeasure>> GetUnitsOfMeasureAsync();
    }
}
