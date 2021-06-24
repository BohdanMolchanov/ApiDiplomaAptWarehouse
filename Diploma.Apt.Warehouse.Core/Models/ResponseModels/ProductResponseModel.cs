using System;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class ProductResponseModel
    {
        public Guid Id { get; set; }
        public string NameUkr { get; set; }
        public string NameEn { get; set; }
        public string ProductType { get; set; }
        public string Description { get; set; }
        public string CreatedAt { get; set; }
        public string Price { get; set; }
        public int TableKey { get; set; }
    }
}