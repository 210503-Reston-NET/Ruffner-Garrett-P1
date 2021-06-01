using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using StoreModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace Data
{
    public class StoreDBContext : IdentityDbContext<ApplicationUser, ApplicationRole , Guid>
    {
        public StoreDBContext(DbContextOptions<StoreDBContext> options) :base(options)
        {

        }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Location>().Property(obj => obj.LocationID).ValueGeneratedOnAdd();
            builder.Entity<Order>().Property(obj => obj.OrderID).ValueGeneratedOnAdd();
            builder.Entity<Item>().Property(obj => obj.ItemID).ValueGeneratedOnAdd();
            builder.Entity<Product>().Property(obj => obj.ProductID).ValueGeneratedOnAdd();
            
            builder.Entity<InventoryItem>().HasKey(t => new {t.ProductID, t.LocationID});
            builder.Entity<InventoryItem>().HasOne<Location>(l => l.location).WithMany(i => i.InventoryItems).HasForeignKey(j => j.LocationID);
            builder.Entity<InventoryItem>().HasOne<Product>(p => p.Product).WithMany(i => i.InventoryItems).HasForeignKey(j => j.ProductID);

            builder.Entity<OrderItem>().HasKey(t => new {t.ProductID, t.OrderID});
            builder.Entity<OrderItem>().HasOne<Order>(o => o.Order).WithMany(i => i.OrderItems).HasForeignKey(j => j.OrderID);
            builder.Entity<OrderItem>().HasOne<Product>(p => p.Product).WithMany(i => i.OrderItems).HasForeignKey(j => j.ProductID);

           
        }

    }
}