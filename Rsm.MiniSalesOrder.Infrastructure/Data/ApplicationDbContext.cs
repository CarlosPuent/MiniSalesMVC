using Microsoft.EntityFrameworkCore;
using Rsm.MiniSalesOrder.Domain.Entities;
using System;

namespace Rsm.MiniSalesOrder.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private static readonly DateTime SeedCreatedAt = new DateTime(2026, 3, 24, 0, 0, 0, DateTimeKind.Utc);

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<SalesOrderItem> SalesOrderItems { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.HasIndex(e => e.Email);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.HasIndex(e => e.Name);
            });

            modelBuilder.Entity<SalesOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.OrderDate).IsRequired();
                entity.Property(e => e.Subtotal).HasPrecision(18, 2);
                entity.Property(e => e.Tax).HasPrecision(18, 2);
                entity.Property(e => e.Total).HasPrecision(18, 2);
                entity.Property(e => e.Notes).HasMaxLength(500);
                entity.HasIndex(e => e.OrderNumber);

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<SalesOrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.LineTotal).HasPrecision(18, 2);

                entity.HasOne(e => e.SalesOrder)
                    .WithMany(o => o.Items)
                    .HasForeignKey(e => e.SalesOrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Shipment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ShippingAddress).IsRequired().HasMaxLength(200);
                entity.Property(e => e.TrackingNumber).HasMaxLength(50);
                entity.Property(e => e.Carrier).HasMaxLength(50);

                entity.HasOne(e => e.SalesOrder)
                    .WithOne(o => o.Shipment)
                    .HasForeignKey<Shipment>(e => e.SalesOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(20);
                entity.Property(e => e.InvoiceDate).IsRequired();
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.HasIndex(e => e.InvoiceNumber);

                entity.HasOne(e => e.SalesOrder)
                    .WithOne(o => o.Invoice)
                    .HasForeignKey<Invoice>(e => e.SalesOrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "Juan",
                    LastName = "Pérez",
                    Email = "juan.perez@example.com",
                    Phone = "7777-1111",
                    CreatedAt = SeedCreatedAt
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "María",
                    LastName = "González",
                    Email = "maria.gonzalez@example.com",
                    Phone = "7777-2222",
                    CreatedAt = SeedCreatedAt
                },
                new Customer
                {
                    Id = 3,
                    FirstName = "Carlos",
                    LastName = "Rodríguez",
                    Email = "carlos.rodriguez@example.com",
                    Phone = "7777-3333",
                    CreatedAt = SeedCreatedAt
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop HP",
                    Description = "Laptop HP 15.6 pulgadas, 8GB RAM, 256GB SSD",
                    UnitPrice = 799.99m,
                    Stock = 10,
                    IsActive = true,
                    CreatedAt = SeedCreatedAt
                },
                new Product
                {
                    Id = 2,
                    Name = "Monitor Dell",
                    Description = "Monitor Dell 24 pulgadas, Full HD",
                    UnitPrice = 249.99m,
                    Stock = 15,
                    IsActive = true,
                    CreatedAt = SeedCreatedAt
                },
                new Product
                {
                    Id = 3,
                    Name = "Teclado Logitech",
                    Description = "Teclado inalámbrico Logitech",
                    UnitPrice = 49.99m,
                    Stock = 20,
                    IsActive = true,
                    CreatedAt = SeedCreatedAt
                },
                new Product
                {
                    Id = 4,
                    Name = "Mouse Microsoft",
                    Description = "Mouse inalámbrico Microsoft",
                    UnitPrice = 29.99m,
                    Stock = 25,
                    IsActive = true,
                    CreatedAt = SeedCreatedAt
                },
                new Product
                {
                    Id = 5,
                    Name = "Impresora Canon",
                    Description = "Impresora multifuncional Canon",
                    UnitPrice = 199.99m,
                    Stock = 8,
                    IsActive = true,
                    CreatedAt = SeedCreatedAt
                }
            );
        }
    }
}