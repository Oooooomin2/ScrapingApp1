using Microsoft.Extensions.Options;
using ScrapingApp.Configs;
using ScrapingApp.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ScrapingApp.Models
{
    public interface ILineModel
    {
        ValueTask SendMessage(string message);

        string CreateMessageBody(IEnumerable<Topic> topics);
    }

    public class LineModel : ILineModel
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly LineSettingsConfig _lineSettingsConfig;

        public LineModel(IHttpClientFactory clientFactory, IOptions<LineSettingsConfig> lineSettingsConfig) 
        {
            _clientFactory = clientFactory;
            _lineSettingsConfig = lineSettingsConfig.Value;
        }

        public string CreateMessageBody(IEnumerable<Topic> topics)
        {
            var sb = new StringBuilder();
            foreach (var topic in topics)
            {
                sb.AppendLine($"{topic.Title} 投稿日付: {topic.CreatedTime}");
                sb.AppendLine($"{topic.Url}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public async ValueTask SendMessage(string message)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "message", message },
                });
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _lineSettingsConfig.AccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            await client.PostAsync("https://notify-api.line.me/api/notify", content);
        }     
    }
}
