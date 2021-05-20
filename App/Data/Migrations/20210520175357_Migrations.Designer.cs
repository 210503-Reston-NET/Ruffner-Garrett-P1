﻿// <auto-generated />
using System;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Data.Migrations
{
    [DbContext(typeof(StoreDBContext))]
    [Migration("20210520175357_Migrations")]
    partial class Migrations
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.6")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("StoreModels.Customer", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("StoreModels.Item", b =>
                {
                    b.Property<int>("ItemID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("LocationID")
                        .HasColumnType("integer");

                    b.Property<int?>("OrderID")
                        .HasColumnType("integer");

                    b.Property<int>("ProductID")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("ItemID");

                    b.HasIndex("LocationID");

                    b.HasIndex("OrderID");

                    b.HasIndex("ProductID");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("StoreModels.Location", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Address")
                        .HasColumnType("text");

                    b.Property<string>("LocationName")
                        .HasColumnType("text");

                    b.HasKey("ID");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("StoreModels.Order", b =>
                {
                    b.Property<int>("OrderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("CustomerID")
                        .HasColumnType("integer");

                    b.Property<int>("LocationID")
                        .HasColumnType("integer");

                    b.Property<double>("Total")
                        .HasColumnType("double precision");

                    b.HasKey("OrderID");

                    b.HasIndex("CustomerID");

                    b.HasIndex("LocationID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("StoreModels.Product", b =>
                {
                    b.Property<int>("ProductID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.HasKey("ProductID");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("StoreModels.Item", b =>
                {
                    b.HasOne("StoreModels.Location", null)
                        .WithMany("InventoryItems")
                        .HasForeignKey("LocationID");

                    b.HasOne("StoreModels.Order", null)
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderID");

                    b.HasOne("StoreModels.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("StoreModels.Order", b =>
                {
                    b.HasOne("StoreModels.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StoreModels.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Customer");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("StoreModels.Location", b =>
                {
                    b.Navigation("InventoryItems");
                });

            modelBuilder.Entity("StoreModels.Order", b =>
                {
                    b.Navigation("OrderItems");
                });
#pragma warning restore 612, 618
        }
    }
}
