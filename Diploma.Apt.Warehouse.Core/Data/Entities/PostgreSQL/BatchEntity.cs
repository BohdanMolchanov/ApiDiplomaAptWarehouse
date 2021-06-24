using System;
using Diploma.Apt.Warehouse.Core.Data.Abstractions;
using Diploma.Apt.Warehouse.Core.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NpgsqlTypes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL
{
    public class BatchEntity : IdentifiedEntity
    {
        private readonly NullableDbDateTime _bestBefore = new NullableDbDateTime();
        public Guid StockId { get; set; }
        public Guid DepartmentId { get; set; }
        public StockEntity Stock { get; set; }
        public int Count { get; set; }
        public bool IsRecieved { get; set; }
        public string ProviderName { get; set; }
        public DateTime? BestBefore {
            get => _bestBefore.Value;
            set => _bestBefore.Value = value;
        }
        
        public DateTime ReceivedAt { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public BatchStates Status { get; set; }
    }
    public class BatchEntityConfiguration : IdentifiedEntityConfiguration<BatchEntity>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<BatchEntity> builder)
        {
            base.ConfigureProperties(builder);
            builder.HasIndex(x => x.Status);
            builder.HasOne(x => x.Stock)
                .WithMany()
                .HasForeignKey(x => x.StockId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}