using ExamSystem.Domain.Entities.Exams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class ExamResultConfiguration : IEntityTypeConfiguration<ExamResult>
    {
        public void Configure(EntityTypeBuilder<ExamResult> builder)
        {
            builder.ToTable("ExamResults");

            builder.HasKey(x => new { x.StudentId, x.ExamId });

            builder.Property(x => x.TotalMark).IsRequired();
            builder.Property(x => x.Score).IsRequired();

            builder.HasOne(x => x.Exam)
                .WithMany(x => x.ExamResults)
                .HasForeignKey(x => x.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Student)
                .WithMany(x => x.ExamResults)
                .HasForeignKey(x => x.StudentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
