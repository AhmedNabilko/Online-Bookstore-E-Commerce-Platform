using E_commerceEntity.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace E_commerceEntity.DataBase
{
    public class E_CommerceContext : IdentityDbContext<APP_User>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Order_Item> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Cart_Item> Cart_Items { get; set; }
        public DbSet<Product> Products { get; set; }

        public E_CommerceContext(DbContextOptions options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            //optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

            //optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=E-commerce;Integrated Security=True;Trust Server Certificate=True");

        }
        protected override void OnModelCreating(ModelBuilder model)
        {

            model.Entity<APP_User>(s =>
            {
                s.HasKey(s => s.Id);
                s.Property(s => s.Id).ValueGeneratedOnAdd();
                s.Property(s => s.FullName).HasMaxLength(80).IsRequired();

                s.HasMany(s => s.Orders).WithOne(s => s.User).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Restrict);
                s.HasMany(s => s.Addresses).WithOne(a => a.User).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
                s.HasOne(s => s.ShoppingCart).WithOne(s => s.User).HasForeignKey<ShoppingCart>(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            });
            model.Entity<Address>(s =>
            {
                s.HasKey(s => s.AddressId);
                s.Property(s => s.AddressId).ValueGeneratedOnAdd();

                s.HasMany(s => s.Orders).WithOne(s => s.Address).HasForeignKey(s => s.AddressId).OnDelete(DeleteBehavior.Restrict);

            });
            model.Entity<Order>(s =>
            {
                s.HasKey(s => s.OrderId);
                s.Property(s => s.OrderId).ValueGeneratedOnAdd();

                s.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");

                s.HasIndex(s => s.OrderNumber, "OrderNumberIDX").IsUnique();
                s.HasMany(s => s.OrderItems).WithOne(s => s.Order).HasForeignKey(s => s.OrderId).OnDelete(DeleteBehavior.Cascade);
            });
            model.Entity<Order_Item>(s =>
            {
                s.Property(s => s.LineTotal).HasColumnType("decimal(18,2)");
                s.Property(s => s.UnitPrice).HasColumnType("decimal(18,2)");
                s.HasKey(s => s.OrderItemId);
                s.Property(s => s.OrderItemId).ValueGeneratedOnAdd();

                s.HasOne(s => s.Product).WithMany(s => s.OrderItems).HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict);
            });
            model.Entity<ShoppingCart>(s =>
            {
                s.HasKey(s => s.Id);
                s.Property(s => s.Id).ValueGeneratedOnAdd();
                s.HasIndex(s => s.UserId, "ShoppingCartUserIdIDX").IsUnique();

                s.HasMany(s => s.Items).WithOne(s => s.ShoppingCart).HasForeignKey(s => s.ShoppingCartId).OnDelete(DeleteBehavior.Cascade);
            });
            model.Entity<Cart_Item>(s =>
            {
                s.HasKey(s => s.Id);
                s.Property(s => s.Id).ValueGeneratedOnAdd();
                s.HasOne(s => s.Product).WithMany(s => s.CartItems).HasForeignKey(s => s.ProductId).OnDelete(DeleteBehavior.Restrict);
            });
            model.Entity<Product>(s =>
            {
                s.HasKey(s => s.ProductId);
                s.Property(s => s.ProductId).ValueGeneratedOnAdd();
                s.HasIndex(s => s.SKU, "ProductSKUIDX").IsUnique();
                s.Property(s => s.Price).HasColumnType("decimal(18,2)");
                s.HasOne(s => s.Category).WithMany(s => s.Products).HasForeignKey(s => s.CategoryId).OnDelete(DeleteBehavior.Restrict);
            });
            model.Entity<Category>(s =>
            {
                s.HasKey(s => s.CategoryId);
                s.Property(s => s.CategoryId).ValueGeneratedOnAdd();
                s.HasIndex(s => s.Name, "CategoryNameIDX").IsUnique();
                s.HasOne(s => s.ParentCategory).WithMany(s => s.SubCategories).HasForeignKey(s => s.ParentCategoryId).OnDelete(DeleteBehavior.Restrict);


            });
            base.OnModelCreating(model);
        }

    }
}
