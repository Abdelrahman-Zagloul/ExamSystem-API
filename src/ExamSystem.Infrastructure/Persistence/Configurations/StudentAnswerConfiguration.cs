using ExamSystem.Domain.Entities.Exams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class StudentAnswerConfiguration : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
            builder.ToTable("StudentAnswers");

            builder.HasKey(x => new { x.StudentId, x.ExamId, x.QuestionId });

            builder.Property(e => e.EvaluationStatus)
                .IsRequired()
                .HasConversion<int>();

            builder.HasOne(x => x.Student)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.StudentId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Exam)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Question)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Option)
                .WithMany(x => x.StudentAnswers)
                .HasForeignKey(x => x.SelectedOptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
