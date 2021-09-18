using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.Extensions.Options;
using ScrapingApp.Configs;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ScrapingApp.Repositories
{
    public interface IGetTopicRepository
    {
        /// <summary>
        /// スクレイピングで対象のトピックを取得します。
        /// </summary>
        /// <returns></returns>
        ValueTask<IHtmlCollection<IElement>> GetTopicDataAsync();
    }
    public class GetTopicRepository : IGetTopicRepository
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ScrapingTargetsConfig _scrapingTargetsConfig;

        public GetTopicRepository(IHttpClientFactory clientFactory, IOptions<ScrapingTargetsConfig> scrapingTargetsConfig)
        {
            _clientFactory = clientFactory;
            _scrapingTargetsConfig = scrapingTargetsConfig.Value;
        }

        public async ValueTask<IHtmlCollection<IElement>> GetTopicDataAsync()
        {
            using var stream = await _clientFactory.CreateClient().GetStreamAsync(new Uri(_scrapingTargetsConfig.Url));
            IHtmlDocument doc = await new HtmlParser().ParseDocumentAsync(stream);
            return doc.GetElementById("").QuerySelectorAll("");
        }
    }
}
