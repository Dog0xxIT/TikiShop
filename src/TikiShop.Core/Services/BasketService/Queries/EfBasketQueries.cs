using TikiShop.Infrastructure;
using TikiShop.Shared.ResponseModels.Basket;

namespace TikiShop.Core.Services.BasketService.Queries
{
    public class EfBasketQueries : IBasketQueries
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<EfBasketQueries> _logger;

        public EfBasketQueries(TikiShopDbContext context, ILogger<EfBasketQueries> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<GetBasketByCustomerIdResponse> GetBasketByCustomerId(int buyerId)
        {
            return new();
            /*
            var result = new GetBasketByCustomerIdResponse
            {
                BuyerId = buyerId,
                Total = 0
            };

            var basket = await _context.Baskets
                .AsNoTracking()
                .Select(b => new { Id = b.Id, BuyerId = b.BuyerId })
                .SingleOrDefaultAsync(b => b.BuyerId == buyerId);

            if (basket is null)
            {
                return result;
            }

            var basketItems = await _context.BasketItems
                .AsNoTracking()
                .Where(bi => bi.BasketId == basket.Id)
                .Include(bi => bi.Product)
                .Include(bi => bi.ProductVariant)
                .Select(bi => new GetBasketByCustomerIdResponse.Item
                {
                    Id = bi.Id,
                    ProductSkuId = bi.ProductSkuId,
                    ProductVariantId = bi.ProductVariantId,
                    Quantity = bi.Quantity,
                    PictureUrl = bi.Product.ThumbnailUrl ?? "",
                    ProductName = bi.Product.Name,
                    ProductVariantName = bi.ProductVariantId != null ? bi.ProductVariant!.Name : "",
                }).ToListAsync();

            result.Items.AddRange(basketItems);

            return result;
            */
        }
    }
}
