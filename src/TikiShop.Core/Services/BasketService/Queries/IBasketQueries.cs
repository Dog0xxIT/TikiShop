﻿namespace TikiShop.Core.Services.BasketService.Queries;

public interface IBasketQueries
{
    Task<dynamic> GetBasketByCustomerId(int buyerId);
}