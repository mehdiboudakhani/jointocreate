namespace JTC
{
    class Program
    {
        static async Task Main()
        {
            IServiceProvider serviceProvider = DependencyInjection.BuildServiceProvider();
            await serviceProvider.GetRequiredService<Bot>().RunAsync();
        }
    }
}
