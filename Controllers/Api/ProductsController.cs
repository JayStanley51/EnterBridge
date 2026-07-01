using EnterBridgePOC.Models.Api;
using EnterBridgePOC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterBridgePOC.Controllers.Api
{
    public class ProductsController(IEnterBridgeApiService api) : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? name,
            [FromQuery] string? description,
            [FromQuery] string? sku,
            [FromQuery] Category? category,
            [FromQuery] ProductSortBy? sortBy,
            [FromQuery] SortDirection? sortDirection) =>
            Ok(await api.GetProductsAsync(pageNumber, pageSize, name, description, sku, category, sortBy, sortDirection));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await api.GetProductByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories() =>
            Ok(await api.GetCategoriesAsync());
    }
}
