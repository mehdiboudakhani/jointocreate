namespace JTC
{
    public static class DependencyInjection
    {
        public static IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates
                }))
                .AddSingleton(serviceProvider => new InteractionService(serviceProvider.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<Bot>()
                .AddSingleton<Statistics>()
                .AddSingleton<ChannelEvent>()
                .AddSingleton<GuildEvent>()
                .AddSingleton<InteractionEvent>()
                .AddSingleton<ModalEvent>()
                .AddSingleton<ReadyEvent>()
                .AddSingleton<UserVoiceStateEvent>()
                .AddSingleton<SecretProvider>()
                .AddSingleton<ComponentFactory>()
                .AddSingleton<ModalFactory>()
                .AddDbContext<Database>()
                .AddScoped<GuildService>()
                .AddScoped<HubService>()
                .AddScoped<SynchronizationService>()
                .AddScoped<TemporaryVoiceChannelService>()
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
