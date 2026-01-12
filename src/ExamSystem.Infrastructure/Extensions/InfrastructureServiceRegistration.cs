using ExamSystem.Application.Contracts.Identity;
using ExamSystem.Application.Settings;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Identity;
using ExamSystem.Infrastructure.Persistence.Contexts;
using ExamSystem.Infrastructure.Persistence.Repositories;
using ExamSystem.Infrastructure.Persistence.SeedData;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
namespace ExamSystem.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddInfrastructureServices(IConfiguration configuration)
            {
                ConfigureDbContextAndIdentity(services, configuration);
                RegisterDependencies(services);
                ConfigureJwtAuthentication(services, configuration);
                return services;
            }
        }

        private static void ConfigureDbContextAndIdentity(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ExamDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ExamDbContext>();
        }
        private static void RegisterDependencies(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();
        }
        private static void ConfigureJwtAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JWTSettings").Get<JWTSettings>() ?? throw new Exception();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecertKey))
                };
            });
        }
    }

}
