namespace JTC.Services
{
    public class GuildService(Database database)
    {
        public async Task<List<GuildEntity>> GetGuildsAsync()
        {
            return await database.Guilds
                .Include(guild => guild.Hubs)
                .ThenInclude(hub => hub.TemporaryVoiceChannels)
                .ToListAsync();
        }

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
