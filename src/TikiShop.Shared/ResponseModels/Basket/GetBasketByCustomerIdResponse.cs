﻿namespace TikiShop.Shared.ResponseModels.Basket
{
    public class GetBasketByCustomerIdResponse
    {
        public int BuyerId { get; set; }
        public int Total { get; set; }
        public List<Item> Items { get; set; } = new();

        public class Item
        {
            public int Id { get; set; }
            public double UnitPrice { get; set; }
            public string PictureUrl { get; set; }
            public int Quantity { get; set; }
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public int? ProductVariantId { get; set; }
            public string? ProductVariantName { get; set; }
        }
    }
}
