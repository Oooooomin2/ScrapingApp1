namespace ScrapingApp.Configs
{
    public sealed class MailSettingsConfig
    {
        public const string SectionName = "MailSettings";

        public string HostName { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string ToName { get; set; }
        public string CcName { get; set; }
        public string BccName { get; set; }
        public string[] TargetsTo { get; set; }
        public string[] TargetsCc { get; set; }
        public string[] TargetsBcc { get; set; }
        public string Subject { get; set; }
        public string DefaultResponse { get; set; }
    }
}
