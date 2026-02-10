namespace JTC.Core
{
    public class Statistics
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly ILogger<Statistics> _logger;
        private readonly Timer _timer;

        public Statistics(DiscordSocketClient discordSocketClient, ILogger<Statistics> logger)
        {
            _discordSocketClient = discordSocketClient;
            _logger = logger;
            _timer = new Timer(LogStats, null, TimeSpan.FromMinutes(1), TimeSpan.FromHours(6));
        }

        private void LogStats(object? state)
        {
            var guildCount = _discordSocketClient.Guilds.Count;
            var userCount = _discordSocketClient.Guilds.Sum(guild => guild.MemberCount);
            _logger.LogInformation("Statistics: {GuildCount} guild(s), {UserCount} user(s).", guildCount, userCount);
        }
    }
}
