namespace JTC.Core
{
    /// <summary>
    /// Tracks and logs basic statistics about the bot.
    /// </summary>
    /// <remarks>
    /// Statistics logs are written every 6 hours, starting 1 minute after the bot's initialization.
    /// </remarks>
    public class Statistics
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger<Statistics> _logger;
        private readonly Timer _timer;

        /// <summary>
        /// Starts periodic logging.
        /// </summary>
        /// <param name="discordSocketClient">The Discord client used to access guilds and members.</param>
        /// <param name="logger">Logger instance for recording statistics.</param>
        public Statistics(DiscordSocketClient discordSocketClient, ILogger<Statistics> logger)
        {
            _discordSocketClient = discordSocketClient;
            _logger = logger;
            _timer = new Timer(LogStats, null, TimeSpan.FromMinutes(1), TimeSpan.FromHours(6));
        }

        /// <summary>
        /// Logs the current number of guilds and users.
        /// </summary>
        /// <param name="state">Optional state object provided by the timer.</param>
        private void LogStats(object? state)
        {
            var guildCount = _discordSocketClient.Guilds.Count;
            var userCount = _discordSocketClient.Guilds.Sum(guild => guild.MemberCount);
            _logger.LogInformation("Statistics: {GuildCount} guild(s), {UserCount} user(s).", guildCount, userCount);
        }
    }
}
