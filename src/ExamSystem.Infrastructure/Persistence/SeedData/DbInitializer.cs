using ExamSystem.Application.Settings;
using ExamSystem.Domain.Constants;
using ExamSystem.Domain.Entities;
using ExamSystem.Domain.Interfaces;
using ExamSystem.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExamSystem.Infrastructure.Persistence.SeedData
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ExamDbContext _context;
        private readonly DefaultUsersSettings _userSetting;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DbInitializer> _logger;
        public DbInitializer(ExamDbContext context, IOptions<DefaultUsersSettings> options, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<DbInitializer> logger)
        {
            _context = context;
            _userSetting = options.Value;
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                await ApplyMigrationAsync();
                await SeedRolesAsync();
                await SeedUsersAsync();

                _logger.LogInformation("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database initialization.");
                throw;
            }
        }
        private async Task ApplyMigrationAsync()
        {
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
                await _context.Database.MigrateAsync();
                _logger.LogInformation("Database migrations applied successfully.");
            }
        }
        private async Task SeedRolesAsync()
        {
            var hasRoles = await _context.Roles.AsNoTracking().AnyAsync();
            if (!hasRoles)
            {
                await _roleManager.CreateAsync(new IdentityRole(Role.Admin));
                await _roleManager.CreateAsync(new IdentityRole(Role.Doctor));
                await _roleManager.CreateAsync(new IdentityRole(Role.Student));

                _logger.LogInformation("Default roles seeded successfully.");
            }
        }
        private async Task SeedUsersAsync()
        {
            var hasUsers = await _context.Users.AsNoTracking().AnyAsync();
            if (!hasUsers)
            {
                await CreateUserAsync("Admin", _userSetting.AdminEmail, _userSetting.AdminPassword, Role.Admin);
                await CreateUserAsync("Doctor", _userSetting.DoctorEmail, _userSetting.DoctorPassword, Role.Doctor);
            }
        }
        private async Task CreateUserAsync(string fullName, string email, string password, string role)
        {
            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = Guid.NewGuid().ToString(),
                Email = email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, role);
            _logger.LogInformation("Default user '{fullName}' with role '{role}' created successfully.", fullName, role);
        }
    }
}
