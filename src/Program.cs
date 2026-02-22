namespace JTC
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Configures dependency injection, applies database migrations and starts the bot.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        static async Task Main()
        {
            IServiceProvider serviceProvider = DependencyInjection.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var database = scope.ServiceProvider.GetRequiredService<Database>();
                database.Database.Migrate();
            }
            await serviceProvider.GetRequiredService<Bot>().RunAsync();
        }
    }
}
