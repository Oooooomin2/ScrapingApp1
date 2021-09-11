using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrapingApp.Configs;
using ScrapingApp.Models;
using ScrapingApp.Repositories;
using ScrapingApp.Services;

namespace ScrapingApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MailSettingsConfig>(Configuration.GetSection(MailSettingsConfig.SectionName));
            services.Configure<ScrapingTargetsConfig>(Configuration.GetSection(ScrapingTargetsConfig.SectionName));
            services.Configure<StorageConfig>(Configuration.GetSection(StorageConfig.SectionName));

            services.AddHttpClient();
            services.AddSingleton<IMailModel, MailModel>();
            services.AddSingleton<IGetTopicRepository, GetTopicRepository>();
            services.AddSingleton<IResolveCsvRepository, ResolveCsvRepository>();
            services.AddSingleton<IResolveTopicsService, ResolveTopicsService>();
            services.AddSingleton<ISystemDate, SystemDate>();

            services.AddHostedService<ScrapingApp>();
        }
    }
}
