using AmazonShop.API.Profiles;
using AmazonShop.Application.Abstraction.IRepositories.IAccountRepo;
using AmazonShop.Application.Abstraction.IRepositories.IBaseRepo;
using AmazonShop.Application.Abstraction.IRepositories.IProductRepo;
using AmazonShop.Application.Abstraction.IRepositories.IUserRepo;
using AmazonShop.Application.Abstraction.IServices.IAccountServices;
using AmazonShop.Application.Abstraction.IServices.IProductServices;
using AmazonShop.Application.Abstraction.IServices.IUserServices;
using AmazonShop.Application.Services.AccountServices;
using AmazonShop.Application.Services.ProductServices;
using AmazonShop.Application.Services.UserServices;
using AmazonShop.Common.Utilities;
using AmazonShop.Domain.Models;
using AmazonShop.Infractructure.Repositories.AccountRepo;
using AmazonShop.Infractructure.Repositories.ProductRepo;
using AmazonShop.Infractructure.Repositories.UserRepo;
using AmazonShop.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace AmazonShop.API.Extension
{
    public static class ProductExtensionRegistrar
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            // Adding the database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default"))
            );

            // Adding AutoMapper
            services.AddAutoMapper(typeof(AmazonShopMappings));

            // Adding repositories and services
            services.AddScoped<IBaseRepository<Admin>, AccountRepository>();
            services.AddScoped<IBaseRepository<User>, UserRepository>();
            services.AddScoped<IProductRepository<Product>, ProductRepository>();
            services.AddScoped<IProductServices, ProductServices>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserServices, UserServices>();

            // Seed roles
            SeedRoles(configuration);

            // Adding authentication
            AddAuthentication(services, configuration);

            // Adding Swagger
            AddSwagger(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            // Add services
            services.AddControllers();
        }

        private static void SeedRoles(IConfiguration configuration)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                Seeder.SeedRoles(context);
            }
        }

        private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"])),
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateIssuer = true,
                    ValidateAudience = true
                };
            });
        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "AmazonShop", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                });
            });
        }
    }
}
