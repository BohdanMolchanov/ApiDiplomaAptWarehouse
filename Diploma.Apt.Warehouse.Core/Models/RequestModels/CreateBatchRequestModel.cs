using System;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class CreateBatchRequestModel
    {
        public Guid? BatchId { get; set; }
        public Guid? StockId { get; set; }
        public int Count { get; set; }
        public string ProviderName { get; set; }
        public DateTime? BestBefore { get; set; }
    }
}