namespace JTC.Services
{
    public class SynchronizationService(DiscordSocketClient discordSocketClient, ILogger<SynchronizationService> logger, IServiceProvider serviceProvider)
    {
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

        private async Task SynchronizeHubsAsync(Database database, HubService hubService)
        {
            var hubs = await hubService.GetHubsAsync();
            foreach (var hub in hubs)
            {
                if (discordSocketClient.GetGuild(hub.GuildId).GetVoiceChannel(hub.VoiceChannelId) is null)
                    await hubService.RemoveHubAsync(hub.VoiceChannelId);
            }
        }

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
