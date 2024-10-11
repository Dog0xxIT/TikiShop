using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TikiShop.Infrastructure.Common;
using TikiShop.Infrastructure.Models;

namespace TikiShop.Infrastructure.SeedData
{
    public static class Seeder
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var textJson = File.ReadAllText("../TikiShop.Infrastructure/SeedData/brands.json");
            var brandList = JsonConvert.DeserializeObject<List<Brand>>(textJson);
            modelBuilder.Entity<Brand>()
                .HasData(brandList!);

            textJson = File.ReadAllText("../TikiShop.Infrastructure/SeedData/categories.json");
            var categoryList = JsonConvert.DeserializeObject<List<Category>>(textJson);
            modelBuilder.Entity<Category>()
                .HasData(categoryList!);

            textJson = File.ReadAllText("../TikiShop.Infrastructure/SeedData/optionTypes.json");
            var optionTypeList = JsonConvert.DeserializeObject<List<OptionType>>(textJson);
            modelBuilder.Entity<OptionType>()
                .HasData(optionTypeList!);

            textJson = File.ReadAllText("../TikiShop.Infrastructure/SeedData/products.json");
            var productList = JsonConvert.DeserializeObject<List<Product>>(textJson);
            productList = productList.Where(p => categoryList
                    .Select(i => i.Id).Contains(p.CategoryId))
                    .ToList();
            modelBuilder.Entity<Product>()
                .HasData(productList!);

            textJson = File.ReadAllText("../TikiShop.Infrastructure/SeedData/productVariants.json");
            var productVariantList = JsonConvert.DeserializeObject<List<ProductVariant>>(textJson);
            productVariantList = productVariantList
                .Where(p => productList.Select(i => i.Id).Contains(p.ProductId))
                .ToList();
            modelBuilder.Entity<ProductVariant>()
                .HasData(productVariantList!);

            modelBuilder.Entity<IdentityRole<int>>()
                .HasData(new List<IdentityRole<int>>
                {
                    new() {Id = 1,Name = RolesConstant.Admin, ConcurrencyStamp = "", NormalizedName = RolesConstant.Admin.ToUpper()},
                    new() {Id = 2,Name = RolesConstant.Customer, ConcurrencyStamp = "", NormalizedName = RolesConstant.Customer.ToUpper()},
                    new() {Id = 3,Name = RolesConstant.Guest, ConcurrencyStamp = "", NormalizedName = RolesConstant.Guest.ToUpper()},
                    new() {Id = 4,Name = RolesConstant.Seller, ConcurrencyStamp = "", NormalizedName = RolesConstant.Seller.ToUpper()},
                });
        }
    }
}
