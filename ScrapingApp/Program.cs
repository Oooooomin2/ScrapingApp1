using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ScrapingApp
{
    class Program
    {
        static async Task Main(string[] args) => await CreateHostBuilder(args).Build().RunAsync();

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    new Startup(context.Configuration).ConfigureServices(services);
                });
    }
}
