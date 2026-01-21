using ExamSystem.Domain.Entities.Exams;
using ExamSystem.Domain.Entities.Questions;
using ExamSystem.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExamSystem.Infrastructure.Persistence.Contexts
{
    public class ExamDbContext : IdentityDbContext
    {
        public ExamDbContext(DbContextOptions<ExamDbContext> options) : base(options) { }

        override protected void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IInfrastructureAssemblyMarker).Assembly);
        }

        public DbSet<Exam> Exams { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<ExamSession> ExamSessions { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
