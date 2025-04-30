namespace Hackathon2025.Services
{
    public class ConfigurationHelper
    {
        public static IConfiguration? _config;

        public static void Initialize(IConfiguration Configuration)
        {
            _config = Configuration;
        }

        public static GithubModelsConfig? GithubModelsConfig => _config!.GetSection("GithubOpenAI").Get<GithubModelsConfig>();
    }
}
