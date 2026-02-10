namespace JTC.Services
{
    public class TemporaryVoiceChannelService(Database database)
    {
        public async Task<List<TemporaryVoiceChannelEntity>> GetTemporaryVoiceChannelsAsync()
        {
            return await database.TemporaryVoiceChannels.ToListAsync();
        }

        public async Task AddTemporaryVoiceChannelAsync(ulong voiceChannelId, ulong hubVoiceChannelId, ulong guildId, ulong ownerId)
        {
            database.TemporaryVoiceChannels.Add(new TemporaryVoiceChannelEntity
            {
                VoiceChannelId = voiceChannelId,
                HubVoiceChannelId = hubVoiceChannelId,
                GuildId = guildId,
                OwnerId = ownerId
            });
            await database.SaveChangesAsync();
        }

        public async Task RemoveTemporaryVoiceChannelAsync(ulong voiceChannelId)
        {
            var temporaryVoiceChannel = await database.TemporaryVoiceChannels.FirstOrDefaultAsync(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId);
            if (temporaryVoiceChannel is null)
                return;
            database.TemporaryVoiceChannels.Remove(temporaryVoiceChannel);
            await database.SaveChangesAsync();
        }

        public async Task<bool> IsTemporaryVoiceChannelAsync(ulong voiceChannelId)
        {
            return await database.TemporaryVoiceChannels.AnyAsync(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId);
        }

        public async Task<ulong> GetOwnerIdAsync(ulong voiceChannelId)
        {
            return await database.TemporaryVoiceChannels
                .Where(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId)
                .Select(temporaryVoiceChannel => (ulong)temporaryVoiceChannel.OwnerId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsOwnerAsync(ulong voiceChannelId, ulong userId)
        {
            var temporaryVoiceChannel = await database.TemporaryVoiceChannels.FirstOrDefaultAsync(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId);
            if (temporaryVoiceChannel is null)
                return false;
            return temporaryVoiceChannel.OwnerId == userId;
        }
    }
}
