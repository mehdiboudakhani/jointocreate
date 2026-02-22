namespace JTC.Services
{
    /// <summary>
    /// Provides methods for managing guilds in the database.
    /// </summary>
    /// <param name="database">The database context used to access guild data.</param>
    public class GuildService(Database database)
    {
        /// <summary>
        /// Retrieves all guilds from the database, including their hubs and temporary voice channels.
        /// </summary>
        /// <returns>A list of <see cref="GuildEntity"/> objects.</returns>
        public async Task<List<GuildEntity>> GetGuildsAsync()
        {
            return await database.Guilds
                .Include(guild => guild.Hubs)
                .ThenInclude(hub => hub.TemporaryVoiceChannels)
                .ToListAsync();
        }

        /// <summary>
        /// Adds a new guild to the database if it doesn't already exist.
        /// </summary>
        /// <param name="guildId">The ID of the guild to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddGuildAsync(ulong guildId)
        {
            if (!await database.Guilds.AnyAsync(guild => guild.GuildId == guildId))
            {
                database.Guilds.Add(new GuildEntity
                {
                    GuildId = guildId
                });
                await database.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes a guild from the database if it exists.
        /// </summary>
        /// <param name="guildId">The ID of the guild to remove.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RemoveGuildAsync(ulong guildId)
        {
            var guild = await database.Guilds.FindAsync(guildId);
            if (guild is not null)
            {
                database.Guilds.Remove(guild);
                await database.SaveChangesAsync();
            }
        }
    }
}
