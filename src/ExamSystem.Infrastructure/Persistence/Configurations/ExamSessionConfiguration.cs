using ExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class ExamSessionConfiguration : IEntityTypeConfiguration<ExamSession>
    {
        public void Configure(EntityTypeBuilder<ExamSession> builder)
        {
            builder.ToTable("ExamSessions");

            builder.HasKey(x => new { x.StudentId, x.ExamId });

            builder.Property(x => x.StartedAt).IsRequired();
            builder.Property(x => x.SubmittedAt).IsRequired(false);

            builder.HasOne(x => x.Exam)
                .WithMany(x => x.ExamSessions)
                .HasForeignKey(x => x.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Student)
                .WithMany(x => x.ExamSessions)
                .HasForeignKey(x => x.StudentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
