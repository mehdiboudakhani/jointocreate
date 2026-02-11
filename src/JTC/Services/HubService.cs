namespace JTC.Services
{
    /// <summary>
    /// Provides methods for managing hubs in the database.
    /// </summary>
    /// <param name="database">The database context used to access hub data.</param>
    public class HubService(Database database)
    {
        /// <summary>
        /// Retrieves a hub from the database based on the provided voice channel ID.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the voice channel.</param>
        /// <returns>The <see cref="HubEntity"/> if found; otherwise, null.</returns>
        public async Task<HubEntity?> GetHubAsync(ulong voiceChannelId)
        {
            return await database.Hubs.FirstOrDefaultAsync(hub => hub.VoiceChannelId == voiceChannelId);
        }

        /// <summary>
        /// Retrieves all hubs from the database, including their temporary voice channels.
        /// </summary>
        /// <returns>A list of <see cref="HubEntity"/> objects.</returns>
        public async Task<List<HubEntity>> GetHubsAsync()
        {
            return await database.Hubs
                .Include(hub => hub.TemporaryVoiceChannels)
                .ToListAsync();
        }

        /// <summary>
        /// Gets the number of hubs in a specific guild.
        /// </summary>
        /// <param name="guildId">The ID of the guild.</param>
        /// <returns>The number of hubs in the guild.</returns>
        public Task<int> GetHubCountAsync(ulong guildId)
        {
            return database.Hubs.CountAsync(hub => hub.GuildId == guildId);
        }

        /// <summary>
        /// Adds a new hub to the database.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the hub's voice channel.</param>
        /// <param name="guildId">The ID of the guild.</param>
        /// <param name="childName">Optional name template for temporary voice channels.</param>
        /// <param name="userLimit">Optional user limit for temporary voice channels.</param>
        /// <param name="temporaryVoiceChannelInterface">Optional flag to enable the management interface for the owner.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
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

        /// <summary>
        /// Removes a hub from the database based on the provided voice channel ID.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the hub's voice channel.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveHubAsync(ulong voiceChannelId)
        {
            var hub = await database.Hubs.FirstOrDefaultAsync(hub => hub.VoiceChannelId == voiceChannelId);
            if (hub is null)
                return;
            database.Hubs.Remove(hub);
            await database.SaveChangesAsync();
        }

        /// <summary>
        /// Checks whether a voice channel is registered as a hub in the database.
        /// </summary>
        /// <param name="voiceChannelId">The ID of the voice channel.</param>
        /// <returns>True if the voice channel is a hub; otherwise, false.</returns>
        public async Task<bool> IsHubAsync(ulong voiceChannelId)
        {
            return await database.Hubs.AnyAsync(hub => hub.VoiceChannelId == voiceChannelId);
        }
    }
}
