namespace Yandex.Direct
{
    public class YapiSettings
    {
        public YapiSettings()
        {
            this.ApiAddress = "https://soap.direct.yandex.ru/json-api/v4/";
            this.Language = YapiLanguage.English;
        }

        public YapiSettings(string certificatePath, string certificatePassword)
            : this()
        {
            this.CertificatePath = certificatePath;
            this.CertificatePassword = certificatePassword;
        }

        public string ApiAddress { get; set; }
        public YapiLanguage Language { get; set; }
        
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
        
        public string MasterToken { get; set; }
        public string DefaultLogin { get; set; }
    }
}