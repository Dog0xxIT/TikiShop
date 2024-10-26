using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TikiShop.Infrastructure.Models;
using TikiShop.Infrastructure.SeedData;

namespace TikiShop.Infrastructure
{
    /*
     * Add migrations using the following command inside the 'Ordering.Infrastructure' project directory:
     * dotnet ef migrations add --startup-project ..\TikiShop.Api\TikiShop.Api.csproj --context TikiShopDbContext [Name]
     * dotnet ef database update --startup-project ..\TikiShop.Api\TikiShop.Api.csproj
     */
    public class TikiShopDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<CardType> CardTypes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentProvider> PaymentProviders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductSku> ProductSkus { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<WishList> WishLists { get; set; }

        public TikiShopDbContext(DbContextOptions<TikiShopDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed();
            base.OnModelCreating(modelBuilder);
        }
    }
}
