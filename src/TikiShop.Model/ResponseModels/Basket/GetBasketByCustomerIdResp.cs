﻿namespace TikiShop.Model.ResponseModels.Basket;

public class GetBasketByCustomerIdResp
{
    public int BuyerId { get; set; }
    public double Total { get; set; }
    public List<Item> Items { get; set; } = new();

    public class Item
    {
        public int Id { get; set; }
        public double UnitPrice { get; set; }
        public string PictureUrl { get; set; }
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
    }
}