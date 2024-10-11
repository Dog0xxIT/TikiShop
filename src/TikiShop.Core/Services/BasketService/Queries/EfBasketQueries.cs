using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TikiShop.Infrastructure;

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

        public async Task<dynamic> GetBasketByCustomerId(int buyerId)
        {
            var result = await _context.Baskets
                .AsNoTracking()
                .Include(b => b.Items)
                .SingleOrDefaultAsync(b => b.BuyerId == buyerId);
            return result;
        }
    }
}
