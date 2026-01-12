using ExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class OptionConfiguration : IEntityTypeConfiguration<Option>
    {
        public void Configure(EntityTypeBuilder<Option> builder)
        {
            builder.ToTable("Options");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.OptionText)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne(e => e.Question)
                .WithMany(d => d.Options)
                .HasForeignKey(e => e.QuestionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
