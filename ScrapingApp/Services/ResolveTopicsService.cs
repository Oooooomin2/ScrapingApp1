using AngleSharp.Dom;
using ScrapingApp.Dtos;
using ScrapingApp.Models;
using ScrapingApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapingApp.Services
{
    public interface IResolveTopicsService
    {
        ValueTask<IEnumerable<Topic>> GetSendTargets();

        IEnumerable<Topic> FormatRawTopics(IHtmlCollection<IElement> elements);

        void UpdateSentTopics(IEnumerable<Topic> topics);
    }

    public class ResolveTopicsService : IResolveTopicsService
    {
        private readonly IResolveCsvRepository _resolveCsvRepository;
        private readonly IGetTopicRepository _getTopicRepository;
        private readonly ISystemDate _systemDate;

        public ResolveTopicsService(
            IResolveCsvRepository resolveCsvRepository,
            IGetTopicRepository getTopicRepository,
            ISystemDate systemDate)
        {
            _resolveCsvRepository = resolveCsvRepository;
            _getTopicRepository = getTopicRepository;
            _systemDate = systemDate;
        }

        public IEnumerable<Topic> FormatRawTopics(IHtmlCollection<IElement> elements)
        {
            var topicList = new List<Topic>();
            var yearStr = _systemDate.GetSystemDate().Year.ToString();
            foreach (var element in elements)
            {
                topicList.Add(new Topic
                {
                    Title = $"{element.QuerySelector("").TextContent}",
                    CreatedTime = DateTime.Parse($"{yearStr}/{element.QuerySelector("").TextContent}"),
                    Url = element.QuerySelector("").GetAttribute("href")
                });
            }
            return topicList;
        }

        public async ValueTask<IEnumerable<Topic>> GetSendTargets()
        {
            var targets = await _getTopicRepository.GetTopicDataAsync();
            return FormatRawTopics(targets).FilterTopicsWithCsv(_resolveCsvRepository.GetTopics());
        }

        public void UpdateSentTopics(IEnumerable<Topic> topics)
        {
            _resolveCsvRepository.UpdateCsv(topics);
        }
    }
    
    public static class ResolveTopicsExtensions
    {
        public static IEnumerable<Topic> FilterTopicsWithCsv(
            this IEnumerable<Topic> topics,
            IEnumerable<Topic> targets)
        {
            return topics.Except(targets);
        }
    }
}
