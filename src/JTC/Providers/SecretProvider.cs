namespace JTC.Providers
{
    public class SecretProvider
    {
        public string DiscordBotToken => GetEnvironmentVariable("JTC_DISCORD_BOT_TOKEN");

        private string GetEnvironmentVariable(string name) =>
            Environment.GetEnvironmentVariable(name) ?? throw new Exception($"Environment variable '{name}' is not set.");
    }
}
