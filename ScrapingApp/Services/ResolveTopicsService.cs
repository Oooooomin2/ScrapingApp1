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
    }

    public class ResolveTopicsService : IResolveTopicsService
    {
        private readonly IResolveCsvService _resolveCsvService;
        private readonly IGetTopicRepository _getTopicRepository;
        private readonly IResolveS3Repository _resolveS3Repository;
        private readonly ISystemDate _systemDate;

        public ResolveTopicsService(
            IResolveCsvService resolveCsvRepository,
            IGetTopicRepository getTopicRepository,
            IResolveS3Repository resolveS3Repository,
            ISystemDate systemDate)
        {
            _resolveCsvService = resolveCsvRepository;
            _getTopicRepository = getTopicRepository;
            _resolveS3Repository = resolveS3Repository;
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
            var csvFile = await _resolveS3Repository.GetCsvFile();
            var topics = await _resolveCsvService.GetTopicsAsync(csvFile);
            return FormatRawTopics(targets).FilterTopicsWithCsv(topics);
        }
    }
    
    public static class ResolveTopicsExtensions
    {
        public static IEnumerable<Topic> FilterTopicsWithCsv(this IEnumerable<Topic> topics, IEnumerable<Topic> targets)
            => topics.Except(targets);
    }
}
