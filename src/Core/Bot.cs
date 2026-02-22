namespace JTC.Core
{
    /// <summary>
    /// Represents the Discord bot.
    /// </summary>
    /// <param name="discordSocketClient">The Discord client used to connect and handle events.</param>
    /// <param name="logger">Logger instance for logging bot activity and errors.</param>
    /// <param name="serviceProvider">Service provider for dependency injection.</param>
    class Bot(DiscordSocketClient discordSocketClient, ILogger<Bot> logger, IServiceProvider serviceProvider)
    {
        /// <summary>
        /// Starts the bot and its services.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RunAsync()
        {
            try
            {
                RegisterEvents();
                _ = serviceProvider.GetRequiredService<Statistics>();
                await serviceProvider.GetRequiredService<InteractionService>().AddModulesAsync(typeof(HubInteraction).Assembly, serviceProvider);
                await discordSocketClient.LoginAsync(TokenType.Bot, serviceProvider.GetRequiredService<SecretProvider>().DiscordBotToken);
                await discordSocketClient.StartAsync();
                logger.LogInformation("The bot has started.");
                await Task.Delay(Timeout.Infinite);
            }
            catch (Exception exception) 
            {
                logger.LogCritical(exception, "The bot failed to start.");
                throw;
            }
        }

        /// <summary>
        /// Registers Discord client events with their handlers.
        /// </summary>
        private void RegisterEvents()
        {
            discordSocketClient.Ready += () => 
                EventWrapper(() => serviceProvider.GetRequiredService<ReadyEvent>().OnReadyAsync(), "ready");
            discordSocketClient.InteractionCreated += (interaction) => 
                EventWrapper(() => serviceProvider.GetRequiredService<InteractionEvent>().OnInteractionCreatedAsync(interaction), "interactionCreated");
            discordSocketClient.JoinedGuild += (guild) => 
                EventWrapper(() => serviceProvider.GetRequiredService<GuildEvent>().OnJoinedGuildAsync(guild), "joinedGuild");
            discordSocketClient.LeftGuild += (guild) => 
                EventWrapper(() => serviceProvider.GetRequiredService<GuildEvent>().OnLeftGuildAsync(guild), "leftGuild");
            discordSocketClient.UserVoiceStateUpdated += (user, before, after) => 
                EventWrapper(() => serviceProvider.GetRequiredService<UserVoiceStateEvent>().OnUserVoiceStateUpdatedAsync(user, before, after), "userVoiceStateUpdated");
            discordSocketClient.ChannelDestroyed += (channel) => 
                EventWrapper(() => serviceProvider.GetRequiredService<ChannelEvent>().OnChannelDestroyedAsync(channel), "channelDestroyed");
            discordSocketClient.ModalSubmitted += (modal) => 
                EventWrapper(() => serviceProvider.GetRequiredService<ModalEvent>().OnModalSubmitted(modal), "modalSubmitted");
        }

        /// <summary>
        /// Executes an event handler safely and logs errors.
        /// </summary>
        /// <param name="handler">The asynchronous event handler to execute.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task EventWrapper(Func<Task> handler, string eventName)
        {
            try
            {
                await handler();
            }
            catch (HttpException httpException) when (httpException.DiscordCode.HasValue && httpException.DiscordCode.Value == DiscordErrorCode.InsufficientPermissions)
            {
                logger.LogWarning("A permission error was detected during the following event: {eventName}.", eventName);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "An error occurred during the following event: {eventName}.", eventName);
            }
        }
    }
}
