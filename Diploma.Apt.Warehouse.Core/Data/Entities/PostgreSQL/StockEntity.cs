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
    public class StockEntity : IdentifiedEntity
    {
        private readonly NullableDbDateTime _orderRepeat = new NullableDbDateTime();
        
        public Guid ProductId { get; set; }
        public ProductEntity Product { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid DepartmentId { get; set; }
        public int Count { get; set; }
        public int MaxCount { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellPrice { get; set; }
        public int? OrderPoint { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StockStates Status { get; set; }
        
        public DateTime? OrderRepeat {
            get => _orderRepeat.Value;
            set => _orderRepeat.Value = value;
        }
        public int? OrderPeriod { get; set; }
    }
    
    public class StockEntityConfiguration : IdentifiedEntityConfiguration<StockEntity>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<StockEntity> builder)
        {
            base.ConfigureProperties(builder);
            builder.HasIndex(x => x.OrganizationId);
            builder.HasIndex(x => x.DepartmentId);
            builder.HasIndex(x => x.Status);
            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .HasPrincipalKey(x => x.Id);
        }
    }
}