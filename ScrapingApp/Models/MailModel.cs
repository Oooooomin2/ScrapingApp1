using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using ScrapingApp.Configs;
using ScrapingApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapingApp.Models
{
    public interface IMailModel
    {
        /// <summary>
        /// メールを送信します。
        /// </summary>
        /// <param name="mimeMessage"></param>
        /// <returns></returns>
        ValueTask SendMail(MimeMessage mimeMessage);

        /// <summary>
        /// メール送信に必要な情報を作成します。
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        MimeMessage CreateMailContext(Mail mail);

        /// <summary>
        /// メールの本文を作成します。
        /// </summary>
        /// <param name="topics"></param>
        /// <returns></returns>
        string CreateMailBody(IEnumerable<Topic> topics);
    }

    public sealed class MailModel : IMailModel
    {
        private readonly MailSettingsConfig _mailSettingsConfig;
        private readonly ILogger<IMailModel> _logger;

        public MailModel(
            IOptions<MailSettingsConfig> mailSettingsConfig,
            ILogger<IMailModel> logger)
        {
            _mailSettingsConfig = mailSettingsConfig.Value;
            _logger = logger;
        }

        public string CreateMailBody(IEnumerable<Topic> topics)
        {
            if (!topics.Any()) return _mailSettingsConfig.DefaultResponse;

            StringBuilder sb = new();
            foreach(var topic in topics)
            {
                sb.AppendLine($"{topic.Title} 投稿日付: {topic.CreatedTime}");
                sb.AppendLine($"{topic.Url}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public MimeMessage CreateMailContext(Mail mail)
        {
            var context = new MimeMessage();
            context.From.Add(new MailboxAddress(_mailSettingsConfig.FromName, _mailSettingsConfig.FromAddress));
            context.To.Add(new MailboxAddress(_mailSettingsConfig.ToName, mail.To));
            context.Bcc.Add(new MailboxAddress(_mailSettingsConfig.BccName, mail.Bcc));
            context.Subject = mail.Subject;
            context.Body = new TextPart(MimeKit.Text.TextFormat.Plain){ Text = mail.Body.ToString() };
            return context;
        }

        public async ValueTask SendMail(MimeMessage mailContext)
        {
            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                _logger.LogInformation("メールを送信します。");
                await client.ConnectAsync(_mailSettingsConfig.HostName, _mailSettingsConfig.Port);
                await client.AuthenticateAsync(_mailSettingsConfig.UserName, _mailSettingsConfig.Password);
                await client.SendAsync(mailContext);
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("メールの送信に失敗しました");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
    }
}
