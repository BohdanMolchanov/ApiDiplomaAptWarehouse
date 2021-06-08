using System;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class ConfirmBatchRequestModel
    {
        public Guid BatchId { get; set; }
        public DateTime? BestBefore { get; set; }
    }
}