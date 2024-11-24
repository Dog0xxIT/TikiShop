using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TikiShop.Infrastructure.Models;
using TikiShop.Infrastructure.SeedData;

namespace TikiShop.Infrastructure;

/*
 * Add migrations using the following command inside the 'Ordering.Infrastructure' project directory:
 * dotnet ef migrations add --startup-project ..\TikiShop.Api\TikiShop.Api.csproj --context TikiShopDbContext [Name]
 * dotnet ef database update --startup-project ..\TikiShop.Api\TikiShop.Api.csproj
 */
public class TikiShopDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public TikiShopDbContext(DbContextOptions<TikiShopDbContext> options) : base(options)
    {
    }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Seed();
        base.OnModelCreating(modelBuilder);
    }

    public override int SaveChanges()
    {
        var errorList = new List<ValidationResult>();

        var entries = ChangeTracker
            .Entries()
            .Where(p => p.State == EntityState.Added || p.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Added)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.SetTimeCreated();
                    baseEntity.SetTimeLastModified();
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entity is BaseEntity baseEntity) baseEntity.SetTimeLastModified();
            }

            Validator.TryValidateObject(entity, new ValidationContext(entity), errorList);
        }

        if (errorList.Any()) throw new Exception(string.Join(", ", errorList.Select(p => p.ErrorMessage)).Trim());

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var errorList = new List<ValidationResult>();

        var entries = ChangeTracker
            .Entries()
            .Where(p => p.State == EntityState.Added || p.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            var entity = entry.Entity;
            if (entry.State == EntityState.Added)
            {
                if (entity is BaseEntity baseEntity)
                {
                    baseEntity.SetTimeCreated();
                    baseEntity.SetTimeLastModified();
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                if (entity is BaseEntity baseEntity) baseEntity.SetTimeLastModified();
            }

            Validator.TryValidateObject(entity, new ValidationContext(entity), errorList);
        }

        if (errorList.Any()) throw new Exception(string.Join(", ", errorList.Select(p => p.ErrorMessage)).Trim());

        return base.SaveChangesAsync();
    }
}