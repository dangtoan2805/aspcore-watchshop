using Microsoft.EntityFrameworkCore;
using aspCore.WatchShop.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace aspCore.WatchShop.EF
{
    public class watchContext : IdentityDbContext
    {
        public watchContext(DbContextOptions<watchContext> options) : base(options) { }

        //DbSets    
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionDetail> PromotionDetails { get; set; }
        public DbSet<TypeWire> TypeWires { get; set; }

        //Configure Property
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Config OrderDetail
            modelBuilder.Entity<OrderDetail>().HasKey(od => new { od.OrderID, od.ProductID });
            modelBuilder.Entity<OrderDetail>().Property(od => od.Promotion).HasColumnType("decimal(2,2)");
            //Config Order
            modelBuilder.Entity<Order>().Property(o => o.BillPromotion).HasColumnType("decimal(2,2)");
            modelBuilder.Entity<Order>().Property(o => o.BillPromotion).HasDefaultValue(0);
            modelBuilder.Entity<Order>().Property(o => o.Tax).HasColumnType("decimal(2,2)");
            modelBuilder.Entity<Order>().Property(o => o.Tax).HasDefaultValue(0);
            modelBuilder.Entity<Order>().Property(o => o.Status).HasDefaultValue(1);
            modelBuilder.Entity<Order>().Property(o => o.DateCreated).HasColumnType("smalldatetime");
            //Config Promotion
            modelBuilder.Entity<Promotion>().Property(p => p.Discount).HasColumnType("decimal(2,2)");
            modelBuilder.Entity<Promotion>().Property(p => p.Status).HasDefaultValue(true);
            modelBuilder.Entity<Promotion>().Property(p => p.ToDate).HasColumnType("smalldatetime");
            modelBuilder.Entity<Promotion>().Property(p => p.FromDate).HasColumnType("smalldatetime");
            //Config Promotion Detail
            modelBuilder.Entity<PromotionDetail>().HasKey(pd => new { pd.ProductID, pd.PromotionID });
            //Config Fee
            modelBuilder.Entity<Fee>().Property(f => f.Tax).HasColumnType("decimal(2,2)");
            //Config Product
            modelBuilder.Entity<Product>().Property(p => p.isShow).HasDefaultValue(true);
            modelBuilder.Entity<Product>().Property(p => p.isDel).HasDefaultValue(false);
            modelBuilder.Entity<Product>().Property(p => p.SaleCount).HasDefaultValue(0);
        }

    }
}
