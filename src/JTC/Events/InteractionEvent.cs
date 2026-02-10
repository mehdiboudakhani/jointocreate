namespace JTC.Events
{
    public class InteractionEvent(DiscordSocketClient discordSocketClient, InteractionService interactionService, IServiceProvider serviceProvider)
    {
        public async Task OnInteractionCreatedAsync(SocketInteraction socketInteraction) =>
            await interactionService.ExecuteCommandAsync(new SocketInteractionContext(discordSocketClient, socketInteraction), serviceProvider);
    }
}
