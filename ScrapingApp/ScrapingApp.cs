using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ScrapingApp.Models;
using ScrapingApp.Repositories;
using ScrapingApp.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScrapingApp
{
    public class ScrapingApp : IHostedService
    {
        private readonly ILineModel _lineModel;
        private readonly IResolveTopicsService _resolveTopicsService;
        private readonly IResolveCsvService _resolveCsvService;
        private readonly IResolveS3Repository _resolveS3Repository;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<ScrapingApp> _logger;

        public ScrapingApp(
            ILineModel lineModel,
            IResolveTopicsService resolveTopicsService,
            IResolveCsvService resolveCsvRepository,
            IResolveS3Repository resolveS3Repository,
            IHostApplicationLifetime hostApplicationLifetime,
            ILogger<ScrapingApp> logger)
        {
            _lineModel = lineModel;
            _resolveTopicsService = resolveTopicsService;
            _resolveCsvService = resolveCsvRepository;
            _resolveS3Repository = resolveS3Repository;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var targetTopics = await _resolveTopicsService.GetSendTargets();
                var lineMessage = _lineModel.CreateMessageBody(targetTopics);
                await _lineModel.SendMessage(lineMessage);
                _resolveCsvService.UpdateCsv(targetTopics);
                await _resolveS3Repository.UpdateCsvFile();
                _hostApplicationLifetime.StopApplication();
            }
            catch(Exception ex)
            {
                _logger.LogError("未処理の例外が発生しました。");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await Task.CompletedTask;
    }
}
