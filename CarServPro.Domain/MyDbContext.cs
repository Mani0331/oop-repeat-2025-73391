using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CarServPro.Domain

{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext()
        {
        }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Admin> Admins { get; set; } = null!;

        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<Mechanic> Mechanics { get; set; } = null!;
        public virtual DbSet<Service> Services { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CarServ;Username=postgres;Password=usman");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.RegistrationNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(c => c.Customer)
                      .WithMany(cu => cu.Cars)
                      .HasForeignKey(c => c.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Mechanic>(entity =>
            {
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Service>(entity =>
            {
                entity.Property(e => e.WorkDescription)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(s => s.Car)
                      .WithMany(c => c.Services)
                      .HasForeignKey(s => s.CarId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(s => s.Mechanic)
                      .WithMany(m => m.Services)
                      .HasForeignKey(s => s.MechanicId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public async Task SeedDataAsync()
        {
            if (!Mechanics.Any() && !Admins.Any())
            {
                var mechanic1 = new Mechanic
                {
                    Name = "Mechanic 1",
                    Email = "mechanic1@carservice.com",
                    Password = "Dorset001^"
                };

                var mechanic2 = new Mechanic
                {
                    Name = "Mechanic 2",
                    Email = "mechanic2@carservice.com",
                    Password = "Dorset001^"
                };

                var admin = new Admin
                {
                    Name = "Admin",
                    Email = "admin@carservice.com",
                    Password = "Dorset001^"
                };

                var customer1 = new Customer
                {
                    Name = "Customer 1",
                    Email = "customer1@carservice.com",
                    Password = "Dorset001^"
                };

                var customer2 = new Customer
                {
                    Name = "Customer 2",
                    Email = "customer2@carservice.com",
                    Password = "Dorset001^"
                };

                Mechanics.Add(mechanic1);
                Mechanics.Add(mechanic2);
                Admins.Add(admin);
                Customers.Add(customer1);
                Customers.Add(customer2);

                await SaveChangesAsync();
            }
        }
    }
}
