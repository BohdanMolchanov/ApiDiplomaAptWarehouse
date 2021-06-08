using System;
using Diploma.Apt.Warehouse.Core.Data.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class StockResponseModel
    {
        public Guid Id { get; set; }
        public int TableKey { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public int Count { get; set; }
        public int? OrderPoint { get; set; }
        public int MaxCount { get; set; }
        public string OrderRepeat { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellPrice { get; set; }
        public string BestBefore { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StockStates Status { get; set; }
    }
}