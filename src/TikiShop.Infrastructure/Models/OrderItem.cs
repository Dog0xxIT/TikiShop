﻿using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class OrderItem : BaseEntity
    {
        public int ProductId { get; set; }
        public double Discount { get; set; }
        public int Units { get; set; }
        public double TotalPrice { get; set; }
        public Product Product { get; set; }
    }
}