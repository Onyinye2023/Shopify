using AmazonShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace AmazonShop.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }


        public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
        {
            public AppDbContext CreateDbContext(string[] args)
            {
                // Build configuration
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                // Get connection string
                var connectionString = configuration.GetConnectionString("Default");

                // Build DbContextOptions
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new AppDbContext(optionsBuilder.Options);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureProductEntity(modelBuilder);
            ConfigureAdminEntity(modelBuilder);
            ConfigureUserEntity(modelBuilder);
            ConfigureRoleEntity(modelBuilder);

            SeedData(modelBuilder);
        }

        private void ConfigureProductEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.PricePerProduct)
                      .HasColumnType("decimal(18, 2)");
            });
        }

        private void ConfigureAdminEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasOne(a => a.Role)
                      .WithMany(r => r.Admins)
                      .HasForeignKey(a => a.RoleId);
            });
        }

        private void ConfigureUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasOne(u => u.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(u => u.RoleId);
            });
        }

        private void ConfigureRoleEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasMany(r => r.Admins)
                      .WithOne(a => a.Role)
                      .HasForeignKey(a => a.RoleId);

                entity.HasMany(r => r.Users)
                      .WithOne(u => u.Role)
                      .HasForeignKey(u => u.RoleId);
            });
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "User" }
            );

            //    modelBuilder.Entity<Admin>().HasData(
            //        new Admin { AdminId = 1, UserName = "admin1", Password = BCrypt.Net.BCrypt.HashPassword("password123"), RoleId = 1 }
            //    );

            //    modelBuilder.Entity<User>().HasData(
            //        new User { UserId = 1, UserName = "user1", Password = BCrypt.Net.BCrypt.HashPassword("password123"), RoleId = 2 }
            //    );
            }
        }
}
