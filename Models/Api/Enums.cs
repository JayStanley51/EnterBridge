using System.Text.Json.Serialization;

namespace EnterBridgePOC.Models.Api
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Category
    {
        Lumber, Plumbing, Electrical, Tools, Paint, Hardware, Garden, Concrete, Insulation, Other
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UnitOfMeasure
    {
        Each, LinearFeet, SquareFeet, CubicFeet, BoardFeet, Pound, Ounce, Gallon, Quart, Pint,
        FluidOunce, SquareYard, CubicYard, Roll, Box, Bundle, Case, Pallet, Ton, Kilogram,
        Gram, Liter, Milliliter, Meter, Centimeter, Millimeter, SquareMeter, CubicMeter
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductSortBy { Name, Description, Sku, Category }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PriceSortBy { DateTime, Amount }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SortDirection { Asc, Desc }
}
