namespace JTC.Events
{
    /// <summary>
    /// Handles events related to Discord interactions.
    /// </summary>
    /// <param name="discordSocketClient">The Discord client used to create the interaction context.</param>
    /// <param name="interactionService">Service for executing interaction commands.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    public class InteractionEvent(DiscordSocketClient discordSocketClient, InteractionService interactionService, IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Called when a Discord interaction is created.
        /// Executes the corresponding command via the interaction service.
        /// </summary>
        /// <param name="socketInteraction">The interaction that was created.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnInteractionCreatedAsync(SocketInteraction socketInteraction) =>
            await interactionService.ExecuteCommandAsync(new SocketInteractionContext(discordSocketClient, socketInteraction), serviceProvider);
    }
}
