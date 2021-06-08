using System;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class CreateStockRequestModel
    {
        public Guid ProductId { get; set; }
        public int MaxCount { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellPrice { get; set; }
        public int? OrderPoint { get; set; }
        public int? OrderPeriod { get; set; }
    }
}