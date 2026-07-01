using EnterBridgePOC.Models.Api;

namespace EnterBridgePOC.Models
{
    public class ProductWithPricesViewModel
    {
        public ProductDto Product { get; set; } = null!;
        public List<PriceDto> Prices { get; set; } = [];
    }
}
