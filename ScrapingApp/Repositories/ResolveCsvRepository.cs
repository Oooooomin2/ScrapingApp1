using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using ScrapingApp.Configs;
using ScrapingApp.Dtos;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ScrapingApp.Repositories
{
    public interface IResolveCsvRepository
    {
        IReadOnlyCollection<Topic> GetTopics();
        void UpdateCsv(IEnumerable<Topic> topicList);
    }

    public class ResolveCsvRepository : IResolveCsvRepository
    {
        private readonly StorageConfig _storageConfig;
        public ResolveCsvRepository(IOptions<StorageConfig> storageConfig)
        {
            _storageConfig = storageConfig.Value;
        }

        public IReadOnlyCollection<Topic> GetTopics()
        {
            using var reader = new StreamReader(_storageConfig.CsvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Topic>().ToList();
        }

        public void UpdateCsv(IEnumerable<Topic> topicList)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var stream = File.Open(_storageConfig.CsvPath, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);
            csv.WriteRecords(topicList);
        }
    }
}
