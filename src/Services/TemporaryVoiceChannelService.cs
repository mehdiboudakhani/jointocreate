namespace JTC.Services
{
    /// <summary>
    /// Provides methods for managing temporary voice channels in the database.
    /// </summary>
    /// <param name="database">The database context used to access temporary voice channel data.</param>
    public class TemporaryVoiceChannelService(Database database)
    {
        /// <summary>
        /// Retrieves all temporary voice channels from the database.
        /// </summary>
        /// <returns>A list of <see cref="TemporaryVoiceChannelEntity"/> objects.</returns>
        public async Task<List<TemporaryVoiceChannelEntity>> GetTemporaryVoiceChannelsAsync()
        {
            return await database.TemporaryVoiceChannels.ToListAsync();
        }

        /// <summary>
        /// Adds a new temporary voice channel to the database.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <param name="hubVoiceChannelId">The ID of the hub voice channel it belongs to.</param>
        /// <param name="guildId">The ID of the guild.</param>
        /// <param name="ownerId">The ID of the user who owns the channel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Removes a temporary voice channel from the database based on its ID.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveTemporaryVoiceChannelAsync(ulong voiceChannelId)
        {
            var temporaryVoiceChannel = await database.TemporaryVoiceChannels.FirstOrDefaultAsync(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId);
            if (temporaryVoiceChannel is null)
                return;
            database.TemporaryVoiceChannels.Remove(temporaryVoiceChannel);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// Checks if a voice channel is registered as a temporary voice channel in the database.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <returns>True if it is a temporary voice channel; otherwise, false.</returns>
        public async Task<bool> IsTemporaryVoiceChannelAsync(ulong voiceChannelId)
        {
            return await database.TemporaryVoiceChannels.AnyAsync(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId);
        }

        /// <summary>
        /// Gets the owner ID of a temporary voice channel based on its voice channel ID.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <returns>The owner's user ID.</returns>
        public async Task<ulong> GetOwnerIdAsync(ulong voiceChannelId)
        {
            return await database.TemporaryVoiceChannels
                .Where(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId)
                .Select(temporaryVoiceChannel => (ulong)temporaryVoiceChannel.OwnerId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Checks whether a user is the owner of a temporary voice channel.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the temporary voice channel.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>True if the user is the owner; otherwise, false.</returns>
        public async Task<bool> IsOwnerAsync(ulong voiceChannelId, ulong userId)
        {
            var temporaryVoiceChannel = await database.TemporaryVoiceChannels.FirstOrDefaultAsync(temporaryVoiceChannel => temporaryVoiceChannel.VoiceChannelId == voiceChannelId);
            if (temporaryVoiceChannel is null)
                return false;
            return temporaryVoiceChannel.OwnerId == userId;
        }
    }
}
