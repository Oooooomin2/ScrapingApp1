using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ScrapingApp
{
    class Program
    {
        static async Task Main() => await CreateHostBuilder().Build().RunAsync();

        private static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    new Startup(context.Configuration).ConfigureServices(services);
                });
    }
}
