using System;
using Diploma.Apt.Warehouse.Core.Data.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class BatchResponseModel
    {
        public Guid Id { get; set; }
        public int TableKey { get; set; }
        public string Name { get; set; }
        public string Details { get; set; }
        public string Provider { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BatchStates Status { get; set; }
        public int Count { get; set; }
        public string CreatedAt { get; set; }
        public string RecievedAt { get; set; }
        public string BestBefore { get; set; }
    }
}