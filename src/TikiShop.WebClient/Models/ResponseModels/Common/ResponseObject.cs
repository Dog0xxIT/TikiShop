﻿namespace TikiShop.WebClient.Models.ResponseModels.Common
{
    public sealed class ResponseObject
    {
        public string Message { get; set; } = "Success";

        public static ResponseObject Succeeded => new();
    }
}