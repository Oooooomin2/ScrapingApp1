using Amazon.S3;
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
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<LineSettingsConfig>(Configuration.GetSection(LineSettingsConfig.SectionName));
            services.Configure<ScrapingTargetsConfig>(Configuration.GetSection(ScrapingTargetsConfig.SectionName));
            services.Configure<LocalStorageConfig>(Configuration.GetSection(LocalStorageConfig.SectionName));
            services.Configure<AWSConfig>(Configuration.GetSection(AWSConfig.SectionName));

            services.AddHttpClient();
            services.AddSingleton<ILineModel, LineModel>();
            services.AddSingleton<IGetTopicRepository, GetTopicRepository>();
            services.AddSingleton<IResolveCsvService, ResolveCsvService>();
            services.AddSingleton<IResolveS3Repository, ResolveS3Repository>();
            services.AddSingleton<IResolveTopicsService, ResolveTopicsService>();
            services.AddSingleton<ISystemDate, SystemDate>();

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            services.AddHostedService<ScrapingApp>();
        }
    }
}
