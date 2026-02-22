namespace JTC.Events
{
    /// <summary>
    /// Handles events related to Discord guilds.
    /// </summary>
    /// <param name="guildService">Service for managing guilds.</param>
    public class GuildEvent(GuildService guildService)
    {
        /// <summary>
        /// Called when the bot joins a guild.
        /// Adds the guild to the database.
        /// </summary>
        /// <param name="socketGuild">The guild that was joined.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnJoinedGuildAsync(SocketGuild socketGuild) =>
            await guildService.AddGuildAsync(socketGuild.Id);

        /// <summary>
        /// Called when the bot leaves a guild.
        /// Removes the guild from the database.
        /// </summary>
        /// <param name="socketGuild">The guild that was left.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnLeftGuildAsync(SocketGuild socketGuild) =>
            await guildService.RemoveGuildAsync(socketGuild.Id);
    }
}
