namespace ExamSystem.Domain.Entities.Users
{
    public class RefreshToken
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string TokenHash { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; private set; }
        public Guid? ReplacedByTokenId { get; private set; }
        public string? CreatedByIp { get; private set; }
        public string? RevokedByIp { get; private set; }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => RevokedAt == null && !IsExpired;

        public string UserId { get; private set; } = null!;
        public ApplicationUser ApplicationUser { get; private set; } = null!;

        private RefreshToken() { }
        public RefreshToken(string userId, string tokenHash, DateTime expiresAt, string? createdByIp)
        {
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = expiresAt;
            CreatedByIp = createdByIp;
        }
        public void Revoke(string? revokedByIp, Guid? replacedByTokenId = null)
        {
            if (RevokedAt != null)
                return;

            RevokedAt = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
            ReplacedByTokenId = replacedByTokenId;
        }
    }
}
