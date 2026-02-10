namespace JTC.Services
{
    public class HubService(Database database)
    {
        public async Task<HubEntity?> GetHubAsync(ulong voiceChannelId)
        {
            return await database.Hubs.FirstOrDefaultAsync(hub => hub.VoiceChannelId == voiceChannelId);
        }

        public async Task<List<HubEntity>> GetHubsAsync()
        {
            return await database.Hubs
                .Include(hub => hub.TemporaryVoiceChannels)
                .ToListAsync();
        }

        public Task<int> GetHubCountAsync(ulong guildId)
        {
            return database.Hubs.CountAsync(hub => hub.GuildId == guildId);
        }

        public async Task AddHubAsync(ulong voiceChannelId, ulong guildId, string? childName, int? userLimit, bool? temporaryVoiceChannelInterface)
        {
            var hub = new HubEntity
            {
                VoiceChannelId = voiceChannelId,
                GuildId = guildId
            };
            if (!string.IsNullOrWhiteSpace(childName))
                hub.ChildName = childName;
            if (userLimit.HasValue)
                hub.UserLimit = userLimit.Value;
            if (temporaryVoiceChannelInterface.HasValue)
                hub.TemporaryVoiceChannelInterface = temporaryVoiceChannelInterface.Value;
            database.Hubs.Add(hub);
            await database.SaveChangesAsync();
        }

        public async Task RemoveHubAsync(ulong voiceChannelId)
        {
            var hub = await database.Hubs.FirstOrDefaultAsync(hub => hub.VoiceChannelId == voiceChannelId);
            if (hub is null)
                return;
            database.Hubs.Remove(hub);
            await database.SaveChangesAsync();
        }

        public async Task<bool> IsHubAsync(ulong voiceChannelId)
        {
            return await database.Hubs.AnyAsync(hub => hub.VoiceChannelId == voiceChannelId);
        }
    }
}
