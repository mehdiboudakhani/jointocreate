namespace JTC.Events
{
    public class ReadyEvent(ILogger<ReadyEvent> logger, IServiceProvider serviceProvider)
    {
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
