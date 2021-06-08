using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diploma.Apt.Warehouse.Core.Data.Abstractions
{
    public class IdentifiedEntity : IdentifiedEntity<Guid>
    {

    } 
    public class IdentifiedEntity<T> : BasicEntity
    {
        public virtual T Id { get; set; }

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TableKey { get; set; }
    }

    public class IdentifiedEntityConfiguration<T>
        : IdentifiedEntityConfiguration<T, Guid> where T : IdentifiedEntity<Guid>
    {
        
    }

    public class IdentifiedEntityConfiguration<E, T> : BasicEntityConfiguration<E> where E : IdentifiedEntity<T>
    {
        /// <summary>
        /// An entity's properties configuration.
        /// </summary>
        /// <param name="builder"></param>
        protected override void ConfigureProperties(EntityTypeBuilder<E> builder)
        {
            builder.HasKey(f => f.Id);
            //builder.HasAlternateKey(f => f.TableKey);
            builder.Property(f => f.CreatedAt)
                .HasDefaultValueSql("now()::timestamp(0) at time zone 'utc'");
        }
    }
}