namespace JTC.Events
{
    public class GuildEvent(GuildService guildService)
    {
        public async Task OnJoinedGuildAsync(SocketGuild socketGuild) =>
            await guildService.AddGuildAsync(socketGuild.Id);

        public async Task OnLeftGuildAsync(SocketGuild socketGuild) =>
            await guildService.RemoveGuildAsync(socketGuild.Id);
    }
}
