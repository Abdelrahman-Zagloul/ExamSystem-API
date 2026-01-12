using ExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.QuestionText)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(e => e.QuestionMark)
                .IsRequired();

            builder.Property(e => e.QuestionType)
                .HasConversion<string>()
                .HasColumnType("NVARCHAR")
                .HasMaxLength(20)
                .IsRequired();

            builder.HasOne(e => e.Exam)
                .WithMany(d => d.Questions)
                .HasForeignKey(e => e.ExamId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.CorrectOption)
                .WithOne(e => e.QuestionAsCorrectOption)
                .HasForeignKey<Question>(e => e.CorrectOptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
