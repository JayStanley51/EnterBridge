using EnterBridgePOC.Models.Api;
using EnterBridgePOC.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterBridgePOC.Controllers.Api
{
    public class PricesController(IEnterBridgeApiService api) : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] int? productId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] double? minAmount,
            [FromQuery] double? maxAmount,
            [FromQuery] UnitOfMeasure? unitOfMeasure,
            [FromQuery] PriceSortBy? sortBy,
            [FromQuery] SortDirection? sortDirection) =>
            Ok(await api.GetPricesAsync(pageNumber, pageSize, productId, startDate, endDate, minAmount, maxAmount, unitOfMeasure, sortBy, sortDirection));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var price = await api.GetPriceByIdAsync(id);
            return price is null ? NotFound() : Ok(price);
        }
    }
}
