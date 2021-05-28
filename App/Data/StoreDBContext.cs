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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Location>().Property(obj => obj.LocationID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Order>().Property(obj => obj.OrderID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Item>().Property(obj => obj.ItemID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Product>().Property(obj => obj.ProductID).ValueGeneratedOnAdd();
            
            modelBuilder.Entity<InventoryItem>().HasKey(t => new {t.ProductID, t.LocationID});
            modelBuilder.Entity<InventoryItem>().HasOne<Location>(l => l.location).WithMany(i => i.InventoryItems).HasForeignKey(j => j.LocationID);
            modelBuilder.Entity<InventoryItem>().HasOne<Product>(p => p.Product).WithMany(i => i.InventoryItems).HasForeignKey(j => j.ProductID);

            modelBuilder.Entity<OrderItem>().HasKey(t => new {t.ProductID, t.OrderID});
            modelBuilder.Entity<OrderItem>().HasOne<Order>(o => o.Order).WithMany(i => i.OrderItems).HasForeignKey(j => j.OrderID);
            modelBuilder.Entity<OrderItem>().HasOne<Product>(p => p.Product).WithMany(i => i.OrderItems).HasForeignKey(j => j.ProductID);

           
        }

    }
}