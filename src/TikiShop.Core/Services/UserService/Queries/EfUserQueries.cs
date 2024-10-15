using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TikiShop.Core.Services.BasketService.Queries;
using TikiShop.Infrastructure;

namespace TikiShop.Core.Services.UserService.Queries
{
    public class EfUserQueries : IUserQueries
    {
        private readonly TikiShopDbContext _context;
        private readonly ILogger<EfUserQueries> _logger;

        public EfUserQueries(TikiShopDbContext context, ILogger<EfUserQueries> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<dynamic> GetAllAddress(int userId)
        {
            var result = await _context.Addresses
                .AsNoTracking()
                .Where(a => a.UserId == userId)
                .ToListAsync();
            return result;
        }
    }
}
