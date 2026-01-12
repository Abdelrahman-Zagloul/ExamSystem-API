using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Persistence.Contexts;
using ExamSystem.Infrastructure.Persistence.Repositories;
using ExamSystem.Infrastructure.Persistence.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        }
    }

}
