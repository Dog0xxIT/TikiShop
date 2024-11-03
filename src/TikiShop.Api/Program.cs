using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TikiShop.Core.Services.EmailService;
using TikiShop.Core.Services.UserService.Queries;
using TikiShop.Core.Services.VnPayService;
using TikiShop.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection(SmtpConfig.SectionName));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(JwtConfig.SectionName));
builder.Services.Configure<VnPayConfig>(builder.Configuration.GetSection(VnPayConfig.SectionName));
builder.Services.AddTransient<IEmailSender<User>, EmailSender>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddSingleton<TikiShopDapperContext>();
builder.Services.AddTransient<ICatalogQueries, EfCatalogQueries>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<IOrderQueries, OrderQueries>();
builder.Services.AddTransient<IBasketQueries, BasketQueries>();
builder.Services.AddTransient<IBasketQueries, EfBasketQueries>();
builder.Services.AddTransient<IUserQueries, EfUserQueries>();
builder.Services.AddTransient<IVnPayService, VnPayService>();

builder.Services.AddMediatR(cfg =>
{
    var assemblies = Assembly.Load("TikiShop.Core");
    cfg.RegisterServicesFromAssemblies(assemblies);
});

builder.Services.AddDbContext<TikiShopDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.SignIn.RequireConfirmedEmail = true;
        options.User.RequireUniqueEmail = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole<int>>()
    .AddSignInManager()
    .AddEntityFrameworkStores<TikiShopDbContext>()
    .AddDefaultTokenProviders();

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["access-token"];
                return Task.CompletedTask;
            },
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy
            .WithOrigins("https://localhost:7258")
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(securityRequirement);
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "TikiShop API",
        Description = "An ASP.NET Core Web API for managing TikiShop items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

builder.Services.AddW3CLogging(logging =>
{
    logging.LoggingFields = W3CLoggingFields.All;
    logging.AdditionalRequestHeaders.Add("x-forwarded-for");
    logging.AdditionalRequestHeaders.Add("x-client-ssl-protocol");
    logging.FileSizeLimit = 5 * 1024 * 1024;
    logging.RetainedFileCountLimit = 2;
    logging.LogDirectory = @"D:\Personal\IT\projects\TikiShop\logs";
    logging.FlushInterval = TimeSpan.FromSeconds(2);
});

var app = builder.Build();

app.UseW3CLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
