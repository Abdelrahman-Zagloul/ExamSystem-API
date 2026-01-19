using ExamSystem.Domain.Entities.Exams;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class ExamConfiguration : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder.ToTable("Exams");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.Description)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.Property(e => e.StartAt).IsRequired();
            builder.Property(e => e.EndAt).IsRequired();


            builder.HasOne(e => e.Doctor)
                .WithMany(d => d.CreatedExams)
                .HasForeignKey(e => e.DoctorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
