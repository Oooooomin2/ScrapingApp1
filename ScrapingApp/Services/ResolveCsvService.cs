using Amazon.S3.Model;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Options;
using ScrapingApp.Configs;
using ScrapingApp.Dtos;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapingApp.Services
{
    public interface IResolveCsvService
    {
        ValueTask<IReadOnlyCollection<Topic>> GetTopicsAsync(GetObjectResponse csvFile);
        void UpdateCsv(IEnumerable<Topic> topicList);
    }

    public class ResolveCsvService : IResolveCsvService
    {
        private readonly LocalStorageConfig _localStorageConfig;
        public ResolveCsvService(IOptions<LocalStorageConfig> storageConfig)
            => _localStorageConfig = storageConfig.Value;

        public async ValueTask<IReadOnlyCollection<Topic>> GetTopicsAsync(GetObjectResponse csvFile)
        {
            var filePathTmp = _localStorageConfig.CsvPath;
            await csvFile.WriteResponseStreamToFileAsync(filePathTmp, false, System.Threading.CancellationToken.None);
            using var reader = new StreamReader(_localStorageConfig.CsvPath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Topic>().ToList();
        }

        public void UpdateCsv(IEnumerable<Topic> topicList)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };
            using var stream = File.Open(_localStorageConfig.CsvPath, FileMode.Append);
            using var writer = new StreamWriter(stream);
            using var csv = new CsvWriter(writer, config);
            csv.WriteRecords(topicList);
        }
    }
}
