namespace JTC.Core
{
    class Bot(DiscordSocketClient discordSocketClient, ILogger<Bot> logger, IServiceProvider serviceProvider)
    {
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
