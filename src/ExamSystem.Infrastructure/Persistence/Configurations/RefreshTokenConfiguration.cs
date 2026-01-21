using ExamSystem.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExamSystem.Infrastructure.Persistence.Configurations
{
    internal class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.CreatedByIp)
                .IsRequired(false)
                .HasMaxLength(45);

            builder.Property(x => x.CreatedByIp)
                .IsRequired(false)
                .HasMaxLength(45);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ReplacedByTokenId)
                .IsRequired(false);

            builder.HasIndex(x => x.TokenHash)
                .IsUnique();

            builder.HasIndex(x => x.UserId);

            builder.HasOne(x => x.ApplicationUser)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(x => x.UserId);
        }
    }
}
