namespace ExamSystem.Application.Settings
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = [];
        public string[] AllowedMethods { get; set; } = [];
        public string[] AllowedHeaders { get; set; } = [];
        public bool AllowCredentials { get; set; }
    }
}
