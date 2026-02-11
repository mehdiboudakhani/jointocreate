namespace JTC.Events
{
    /// <summary>
    /// Handles the Discord client's ready event.
    /// </summary>
    /// <param name="logger">Logger for recording bot readiness messages.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    public class ReadyEvent(ILogger<ReadyEvent> logger, IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Called when the Discord client is ready.
        /// Registers slash commands and performs synchronization tasks.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task OnReadyAsync()
        {
            using var scope = serviceProvider.CreateScope();
            var interactionService = scope.ServiceProvider.GetRequiredService<InteractionService>();
            var synchronizationService = scope.ServiceProvider.GetRequiredService<SynchronizationService>();
            await interactionService.RegisterCommandsGloballyAsync();
            logger.LogInformation("Slash commands registered successfully.");
            await synchronizationService.Synchronization();
            logger.LogInformation("The bot is ready.");
        }
    }
}
