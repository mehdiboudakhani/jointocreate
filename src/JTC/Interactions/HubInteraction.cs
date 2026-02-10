namespace JTC.Interactions
{
    public class HubInteraction(HubService hubService, ILogger<HubInteraction> logger) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("new-hub", "Create a hub.")]
        public async Task AddHubAsync(
            [Summary("child-name", "Name given to temporary voice channels. Available variable: {user}.")]
            [MinLength(1)]
            [MaxLength(50)]
            string? childName = null,
            [Summary("user-limit", "Limit the number of users in temporary voice channels. 0 for unlimited.")]
            [MinValue(0)]
            [MaxValue(99)]
            int? userLimit = null,
            [Summary("interface", "Enable/Disable the temporary voice channel management interface for the owner.")]
            bool? @interface = null)
        {
            try
            {
                int hubCount = await hubService.GetHubCountAsync(Context.Guild.Id);
                if (hubCount >= 5)
                {
                    await RespondAsync("A guild can only have 5 hubs.", ephemeral: true);
                    return;
                }
                var hub = await Context.Guild.CreateVoiceChannelAsync(name: "【➕】Create voice channel", props =>
                {
                    props.CategoryId = null;
                });
                await hubService.AddHubAsync(hub.Id, Context.Guild.Id, childName, userLimit, @interface);
                await RespondAsync("A new hub has been successfully created!", ephemeral: true);
            }
            catch (HttpException httpException) when (httpException.DiscordCode.HasValue && httpException.DiscordCode.Value == DiscordErrorCode.InsufficientPermissions)
            {
                logger.LogWarning("A permission error was detected during a slash command: /new-hub");
            }
        }
    }
}
