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

        // public DbSet<Customer> Customers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Item> OrderItems { get; set; }
        public DbSet<Item> InventoryItems { get; set; }
        
        public DbSet<Product> Products { get; set; }

        // public DbSet<ApplicationUser> Users {get; set;}

        // public DbSet<ApplicationUser> Users { get; set;0}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.Entity<Customer>().Property(obj => obj.CustomerID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Location>().Property(obj => obj.LocationID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Order>().Property(obj => obj.OrderID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Item>().Property(obj => obj.ItemID).ValueGeneratedOnAdd();
            modelBuilder.Entity<Product>().Property(obj => obj.ProductID).ValueGeneratedOnAdd();
           
        }

    }
}