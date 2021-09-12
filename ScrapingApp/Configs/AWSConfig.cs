namespace ScrapingApp.Configs
{
    public sealed class AWSConfig
    {
        public const string SectionName = "AWS";

        public IAM IAM { get; set; }
        public S3 S3 { get; set; }
    }

    public sealed class IAM
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }

    public sealed class S3
    {
        public string BacketName { get; set; }
        public string Key { get; set; }
    }
}
