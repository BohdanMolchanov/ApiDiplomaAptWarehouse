using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diploma.Apt.Warehouse.Core.Data.Abstractions
{
    public class BasicEntity
    {
        private readonly NullableDbDateTime _lastUpdatedAt = new NullableDbDateTime(DateTime.Now);
        private readonly DbDateTime _createdAt = new DbDateTime(DateTime.Now);
        
        /// <summary>
        /// Incremental key.
        /// </summary>
        // [DatabaseGenerated(DatabaseGeneratedOption.Identity), Key]
        // public virtual int TableKey { get; set; }
        [Required]
        public DateTime CreatedAt
        {
            get => _createdAt.Value;
            set => _createdAt.Value = value;
        }
        /// <inheritdoc/>
        public DateTime? LastUpdatedAt
        {
            get => _lastUpdatedAt.Value;
            set => _lastUpdatedAt.Value = value;
        }
    }
    
    public class BasicEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : BasicEntity
    {
        public void Configure(EntityTypeBuilder<T> builder)
        {
            ConfigureProperties(builder);
            ConfigureDataSeeding(builder);
        }

        /// <summary>
        /// An entity's properties configuration.
        /// </summary>
        /// <param name="builder"></param>
        protected virtual void ConfigureProperties(EntityTypeBuilder<T> builder)
        {
            //builder.HasKey(f => f.TableKey);
            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("now()::timestamp(0) at time zone 'utc'");
        }
        
        protected virtual void ConfigureDataSeeding(EntityTypeBuilder<T> builder)
        {
            
        }
    }
}