using Diploma.Apt.Warehouse.Core.Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NpgsqlTypes;

namespace Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL
{
    public class ProductEntity : IdentifiedEntity
    {
        public string NameUkr { get; set; }
        public string NameEn { get; set; }
        public string ProductType { get; set; }
        public string Description { get; set; }
        public NpgsqlTsVector SearchVector { get; set; }
    }
    
    public class ProductEntityConfiguration : IdentifiedEntityConfiguration<ProductEntity>
    {
        protected override void ConfigureProperties(EntityTypeBuilder<ProductEntity> builder)
        {
            base.ConfigureProperties(builder);
            builder.HasIndex(f => f.SearchVector)
                .HasMethod("GIN");
        }
    }
}