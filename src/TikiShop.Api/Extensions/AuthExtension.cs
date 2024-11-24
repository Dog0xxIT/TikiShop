using System.Text;
using Microsoft.IdentityModel.Tokens;
using TikiShop.Model.Configurations;

namespace TikiShop.Api.Extensions
{
    public static class AuthExtension
    {
        public static void AddProjectAuth(this WebApplicationBuilder builder)
        {
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(IdentityConstants.ExternalScheme)
                .AddJwtBearer(options =>
                {
                    var jwtConfig = new JwtConfig();
                    builder.Configuration.GetSection(JwtConfig.SectionName).Bind(jwtConfig);

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["access-token"];
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddGoogle(options =>
                {
                    var ggOAuthConfig = builder.Configuration.GetSection("GoogleOAuth");
                    options.ClientId = ggOAuthConfig["client_id"]!;
                    options.ClientSecret = ggOAuthConfig["client_secret"]!;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                    options.CallbackPath = "/signin-google";
                });
        }
    }
}
