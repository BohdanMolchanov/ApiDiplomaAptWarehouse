using Diploma.Apt.Warehouse.Core.Data.Entities.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace Diploma.Apt.Warehouse.Core.Data
{
    public class WarehouseContext : DbContext
    {
        public DbSet<BatchEntity> Batches { get; set; }
        public DbSet<ProductEntity> Products { get; set; }
        public DbSet<StockEntity> Stocks { get; set; }
        
        public WarehouseContext(DbContextOptions<WarehouseContext> options)
            : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseIdentityColumns();
            modelBuilder.ApplyConfiguration(new ProductEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BatchEntityConfiguration());
            modelBuilder.ApplyConfiguration(new StockEntityConfiguration());
        }
    }
}