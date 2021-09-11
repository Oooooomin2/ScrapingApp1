using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ScrapingApp.Configs;
using ScrapingApp.Dtos;
using ScrapingApp.Models;
using ScrapingApp.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScrapingApp
{
    public class ScrapingApp : IHostedService
    {
        private readonly IMailModel _mailModel;
        private readonly IResolveTopicsService _resolveTopicsService;
        private readonly MailSettingsConfig _mailSettingsConfig;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<ScrapingApp> _logger;
        public ScrapingApp(
            IMailModel mailModel,
            IResolveTopicsService resolveTopicsService,
            IOptions<MailSettingsConfig> mailSettingsConfig,
            IHostApplicationLifetime hostApplicationLifetime,
            ILogger<ScrapingApp> logger)
        {
            _mailModel = mailModel;
            _resolveTopicsService = resolveTopicsService;
            _mailSettingsConfig = mailSettingsConfig.Value;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                var targetTopics = await _resolveTopicsService.GetSendTargets();
                var mailBody = _mailModel.CreateMailBody(targetTopics);
                for (var i = 0; i < _mailSettingsConfig.TargetsTo.Length; i++)
                {
                    var mail = new Mail
                    {
                        To = _mailSettingsConfig.TargetsTo[i],
                        Bcc = _mailSettingsConfig.TargetsBcc[0],
                        Subject = _mailSettingsConfig.Subject,
                        Body = mailBody
                    };
                    var context = _mailModel.CreateMailContext(mail);
                    await _mailModel.SendMail(context);
                    await Task.Delay(2000);
                }
                _resolveTopicsService.UpdateSentTopics(targetTopics);
                _hostApplicationLifetime.StopApplication();
            }
            catch(Exception ex)
            {
                _logger.LogError("未処理の例外が発生しました。");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }
    }
}
