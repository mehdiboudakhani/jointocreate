namespace JTC
{
    /// <summary>
    /// Provides dependency injection setup.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Builds and configures the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <returns>A configured <see cref="IServiceProvider"/> instance.</returns>
        public static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()

                // Discord
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates
                }))
                .AddSingleton(serviceProvider => new InteractionService(serviceProvider.GetRequiredService<DiscordSocketClient>()))

                // Core
                .AddSingleton<Bot>()
                .AddSingleton<Statistics>()

                // Events
                .AddSingleton<ChannelEvent>()
                .AddSingleton<GuildEvent>()
                .AddSingleton<InteractionEvent>()
                .AddSingleton<ModalEvent>()
                .AddSingleton<ReadyEvent>()
                .AddSingleton<UserVoiceStateEvent>()

                // Providers
                .AddSingleton<SecretProvider>()

                // Factories
                .AddSingleton<ComponentFactory>()
                .AddSingleton<ModalFactory>()

                // Data
                .AddDbContext<Database>()

                // Services
                .AddScoped<GuildService>()
                .AddScoped<HubService>()
                .AddScoped<SynchronizationService>()
                .AddScoped<TemporaryVoiceChannelService>()

                // Logging
                .AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog(new LoggerConfiguration()
                        .MinimumLevel.Information()
                        .WriteTo.File(PathProvider.GetLogPath(), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
                        .CreateLogger());
                })

                .BuildServiceProvider();
        }
    }
}
