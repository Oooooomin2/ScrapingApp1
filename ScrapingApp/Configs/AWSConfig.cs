namespace ScrapingApp.Configs
{
    public sealed class AWSConfig
    {
        public const string SectionName = "AWS";

        public S3 S3 { get; set; }
    }

    public sealed class S3
    {
        public string BacketName { get; set; }
        public string Key { get; set; }
    }
}
