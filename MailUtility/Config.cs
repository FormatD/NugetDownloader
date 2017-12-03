namespace MailUtility
{
    public class Config
    {
        public string SendFrom { get; set; } = "sample@live.cn";

        public string SendTo { get; set; } = "dengqianjun@huawei.com";

        public int MaxSize { get; set; } = 9000000;
        public string SmtpServerAddress { get; set; } = "smtp-mail.outlook.com";
        public int SmtpServerPort { get; set; } = 587;
        public string UserName { get; set; } = "sample@live.cn";
        public string UserPass { get; set; } = "";

        public string PackagesSavePath { get; set; } = @"D:\Microsoft.Nuget.Packages\";

    }
}