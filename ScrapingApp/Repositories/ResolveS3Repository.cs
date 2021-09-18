using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using ScrapingApp.Configs;
using System.Threading.Tasks;

namespace ScrapingApp.Repositories
{
    public interface IResolveS3Repository
    {
        /// <summary>
        /// S3からCSVファイルを取得します。
        /// </summary>
        /// <returns></returns>
        ValueTask<GetObjectResponse> GetCsvFile();

        /// <summary>
        /// S3のCSVファイルを更新します。
        /// </summary>
        /// <returns></returns>
        ValueTask UpdateCsvFile();
    }

    public class ResolveS3Repository : IResolveS3Repository
    {
        private readonly AWSConfig _awsConfig;
        private readonly LocalStorageConfig _localStorageConfig;
        private readonly IAmazonS3 _amazonS3Client;

        public ResolveS3Repository(
            IOptions<AWSConfig> awsConfig,
            IOptions<LocalStorageConfig> localStorageConfig,
            IAmazonS3 amazonS3Client)
        {
            _awsConfig = awsConfig.Value;
            _localStorageConfig = localStorageConfig.Value;
            _amazonS3Client = amazonS3Client;
        }

        public async ValueTask<GetObjectResponse> GetCsvFile()
            => await _amazonS3Client.GetObjectAsync(_awsConfig.S3.BacketName, _awsConfig.S3.Key);

        public async ValueTask UpdateCsvFile()
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _awsConfig.S3.BacketName,
                Key = _awsConfig.S3.Key,
                FilePath = _localStorageConfig.CsvPath
            };
            await _amazonS3Client.PutObjectAsync(putRequest);
        }
    }
}
