using ExamSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("AspNetUsers");
            builder.HasKey(t => t.Id);

            builder.Property(u => u.FullName)
                .HasColumnType("NVARCHAR")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.HasDiscriminator<string>("UserType")
                   .HasValue<Doctor>("Doctor")
                   .HasValue<Student>("Student");
        }
    }
}
