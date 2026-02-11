namespace JTC.Services
{
    /// <summary>
    /// Provides methods to synchronize the database with the current state of the Discord server.
    /// </summary>
    /// <param name="discordSocketClient">The Discord client used to access guilds and voice channels.</param>
    /// <param name="logger">Logger for recording synchronization messages.</param>
    /// <param name="serviceProvider">Service provider used for dependency injection and to resolve scoped services.</param>
    public class SynchronizationService(DiscordSocketClient discordSocketClient, ILogger<SynchronizationService> logger, IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Performs a full synchronization of the database with the current Discord state.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Synchronization()
        {
            using var scope = serviceProvider.CreateScope();
            var database = scope.ServiceProvider.GetRequiredService<Database>();
            logger.LogInformation("Starting database synchronization...");
            await SynchronizeGuildsAsync(database, serviceProvider.GetRequiredService<GuildService>());
            await SynchronizeHubsAsync(database, serviceProvider.GetRequiredService<HubService>());
            await SynchronizeTemporaryVoiceChannelsAsync(database, serviceProvider.GetRequiredService<TemporaryVoiceChannelService>());
            logger.LogInformation("Database synchronization completed.");
        }

        /// <summary>
        /// Synchronizes the guilds in the database with the guilds the bot is currently in.
        /// Removes guilds that no longer exist and adds missing guilds.
        /// </summary>
        /// <param name="database">The database context.</param>
        /// <param name="guildService">Service for managing guilds.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SynchronizeGuildsAsync(Database database, GuildService guildService)
        {
            var guilds = await guildService.GetGuildsAsync();
            foreach (var guild in guilds)
            {
                if (discordSocketClient.GetGuild(guild.GuildId) is null)
                    await guildService.RemoveGuildAsync(guild.GuildId);
            }
            foreach (var guild in discordSocketClient.Guilds)
            {
                if (!guilds.Select(guild => guild.GuildId).ToHashSet().Contains(guild.Id))
                    await guildService.AddGuildAsync(guild.Id);
            }
        }

        /// <summary>
        /// Synchronizes the hubs in the database with the existing voice channels in Discord.
        /// Removes hubs whose voice channels no longer exist.
        /// </summary>
        /// <param name="database">The database context.</param>
        /// <param name="hubService">Service for managing hubs.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SynchronizeHubsAsync(Database database, HubService hubService)
        {
            var hubs = await hubService.GetHubsAsync();
            foreach (var hub in hubs)
            {
                if (discordSocketClient.GetGuild(hub.GuildId).GetVoiceChannel(hub.VoiceChannelId) is null)
                    await hubService.RemoveHubAsync(hub.VoiceChannelId);
            }
        }

        /// <summary>
        /// Synchronizes temporary voice channels in the database with Discord.
        /// Removes temporary voice channels that no longer exist or are empty, and deletes their corresponding Discord voice channels if needed.
        /// </summary>
        /// <param name="database">The database context.</param>
        /// <param name="temporaryVoiceChannelService">Service for managing temporary voice channels.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SynchronizeTemporaryVoiceChannelsAsync(Database database, TemporaryVoiceChannelService temporaryVoiceChannelService)
        {
            var temporaryVoiceChannels = await temporaryVoiceChannelService.GetTemporaryVoiceChannelsAsync();
            foreach (var temporaryVoiceChannel in temporaryVoiceChannels)
            {
                var voiceChannel = discordSocketClient.GetGuild(temporaryVoiceChannel.GuildId).GetVoiceChannel(temporaryVoiceChannel.VoiceChannelId);
                if (voiceChannel is null)
                {
                    await temporaryVoiceChannelService.RemoveTemporaryVoiceChannelAsync(temporaryVoiceChannel.VoiceChannelId);
                    continue;
                }
                var humans = voiceChannel.Users
                    .Where(user => !user.IsBot && user.Id != discordSocketClient.CurrentUser.Id)
                    .ToList();
                if (humans.Count == 0)
                {
                    if (voiceChannel.ConnectedUsers.Any())
                        return;
                    await voiceChannel.DeleteAsync();
                    await temporaryVoiceChannelService.RemoveTemporaryVoiceChannelAsync(temporaryVoiceChannel.VoiceChannelId);
                }
            }
        }
    }
}
