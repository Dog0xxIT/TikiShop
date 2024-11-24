using TikiShop.Core.Services.EmailService;
using TikiShop.Core.Services.LogService;
using TikiShop.Model.Configurations;

namespace TikiShop.Api.Extensions;

public static class ProjectExtension
{
    public static void AddProjectConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpConfig>(configuration.GetSection(SmtpConfig.SectionName));
        services.Configure<JwtConfig>(configuration.GetSection(JwtConfig.SectionName));
        services.Configure<VnPayConfig>(configuration.GetSection(VnPayConfig.SectionName));
    }

    public static void AddProjectServices(this IServiceCollection services)
    {
        services.AddSingleton<TikiShopDapperContext>();
        services.AddScoped(typeof(ILogService<>), typeof(LogService<>));
        services.AddTransient<ICatalogQueries, EfCatalogQueries>();
        services.AddTransient<IOrderQueries, OrderQueries>();
        services.AddTransient<IBasketQueries, BasketQueries>();
        services.AddTransient<IBasketQueries, EfBasketQueries>();
        services.AddTransient<IUserQueries, EfUserQueries>();
        services.AddTransient<IVnPayService, VnPayService>();
        services.AddTransient<IEmailSender<User>, EmailSender>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddMediatR(cfg =>
        {
            var assemblies = Assembly.Load("TikiShop.Core");
            cfg.RegisterServicesFromAssemblies(assemblies);
        });
    }
}