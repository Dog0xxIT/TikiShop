using Microsoft.AspNetCore.HttpLogging;

namespace TikiShop.Api.Extensions
{
    public static class LoggingExtension
    {
        public static void AddProjectW3CLogging(this IServiceCollection services)
        {
            services.AddW3CLogging(options =>
            {
                options.LoggingFields = W3CLoggingFields.All;
                options.AdditionalRequestHeaders.Add("x-forwarded-for");
                options.AdditionalRequestHeaders.Add("x-client-ssl-protocol");
                options.FileSizeLimit = 5 * 1024 * 1024;
                options.RetainedFileCountLimit = 2;
                options.LogDirectory = @"D:\Personal\IT\projects\TikiShop\logs";
                options.FlushInterval = TimeSpan.FromSeconds(2);
            });
        }
    }
}
