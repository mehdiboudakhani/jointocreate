namespace JTC
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Configures dependency injection and starts the bot.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        static async Task Main()
        {
            IServiceProvider serviceProvider = DependencyInjection.BuildServiceProvider();
            await serviceProvider.GetRequiredService<Bot>().RunAsync();
        }
    }
}
