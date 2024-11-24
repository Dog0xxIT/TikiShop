using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace TikiShop.Infrastructure;

public class TikiShopDapperContext
{
    private readonly string _connectionString;

    public TikiShopDapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ??
                            throw new ArgumentNullException(nameof(_connectionString));
    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}