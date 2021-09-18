namespace ScrapingApp.Configs
{
    public sealed class LineSettingsConfig
    {
        public const string SectionName = "LineSettings";

        public string AccessToken { get; set; }
        public string DefaultResponse { get; set; }
    }
}
