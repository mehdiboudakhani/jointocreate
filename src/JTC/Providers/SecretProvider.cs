namespace JTC.Providers
{
    /// <summary>
    /// Provides access to secrets stored in environment variables.
    /// </summary>
    public class SecretProvider
    {
        /// <summary>
        /// Gets the Discord bot token from the environment variables.
        /// </summary>
        public string DiscordBotToken => GetEnvironmentVariable("JTC_DISCORD_BOT_TOKEN");

        /// <summary>
        /// Retrieves the values of the specified environment variable.
        /// </summary>
        /// <param name="name">The name of the environment variable.</param>
        /// <returns>The value of the environment variable.</returns>
        /// <exception cref="Exception">Thrown if the environment variable is not set.</exception>
        private string GetEnvironmentVariable(string name) =>
            Environment.GetEnvironmentVariable(name) ?? throw new Exception($"Environment variable '{name}' is not set.");
    }
}
