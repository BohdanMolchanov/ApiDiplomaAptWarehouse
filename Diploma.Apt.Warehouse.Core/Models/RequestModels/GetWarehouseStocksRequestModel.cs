using System;

namespace Diploma.Apt.Warehouse.Core.Models.RequestModels
{
    public class GetWarehouseStocksRequestModel
    {
        public int Skip { get; set; }
        public string Status { get; set; }
        public string Search { get; set; }
        public int Limit { get; set; }
        public Guid DepartmentId { get; set; }
    }
}