namespace ExamSystem.Application.Settings
{
    public class RefreshTokenSettings
    {
        public int RefreshTokenLifetimeDays { get; set; }
        public string RefreshTokenHashKey { get; set; } = null!;
    }
}
