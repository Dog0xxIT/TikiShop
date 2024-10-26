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
            _logger.LogInformation($"Starting to retrieve basket for BuyerId: {buyerId}"); // Thêm logging với string interpolation

            var result = await _context.Baskets
                .AsNoTracking()
                .Select(basket => 
                    new GetBasketByCustomerIdResponse
                    {
                        BuyerId = basket.BuyerId,
                        Total = basket.Total,
                        Items = basket.Items.Select(bi => new GetBasketByCustomerIdResponse.Item
                        {
                            Id = bi.Id,
                            Quantity = bi.Quantity,
                            PictureUrl = bi.ProductSku.Product.ThumbnailUrl,
                            UnitPrice = bi.ProductSku.Price,
                            ProductId = bi.ProductSkuId,
                            Sku = bi.ProductSku.Sku,
                        }).ToList(),
                    })
                .SingleOrDefaultAsync(basket => basket.BuyerId == buyerId);

            if (result != null)
            {
                _logger.LogInformation($"Successfully retrieved basket for BuyerId: {buyerId}"); // Thêm logging với string interpolation
            }
            else
            {
                _logger.LogWarning($"No basket found for BuyerId: {buyerId}"); // Thêm logging cho trường hợp không tìm thấy
            }

            return result ?? new(); 
        }
    }
}
