namespace ExamSystem.Application.Settings
{
    public class JWTSettings
    {
        public string SecertKey { get; set; } = null!;
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public int DurationInMinutes { get; set; }
    }
}
