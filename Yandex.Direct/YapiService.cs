using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Yandex.Direct
{
    public partial class YapiService
    {
        public YapiSettings Setting { get; private set; }

        private JsonSerializerSettings JsonSettings { get; set; }

        public YapiService(YapiSettings settings)
        {
            Contract.Requires(settings != null);
            this.Setting = settings;
            this.JsonSettings =
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Converters = { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" } }
                };

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;
        }

        /// <summary>
        /// Создать клиента для Яндекс-Директа, используя файл конфигурации (секция yandex.direct)
        /// </summary>
        public YapiService()
            : this(YapiSettings.FromConfiguration())
        { }

        /// <summary>
        /// Создать клиента для Яндекс-Директа, указав путь к файлу сертификата и пароль
        /// </summary>
        public YapiService(string certificatePath, string certificatePassword)
            : this(new YapiSettings(certificatePath, certificatePassword))
        { }

        #region Отправка запросов к API Яндекса

        private string HttpRequest(string method, string parametersJson, bool sign = false)
        {
            Contract.Requires(!sign || !string.IsNullOrWhiteSpace(this.Setting.MasterToken), "Financial operations require MasterToken and Login to be set");

            var request = (HttpWebRequest)WebRequest.Create(this.Setting.ApiAddress);
            request.ClientCertificates.Add(new X509Certificate2(this.Setting.CertificatePath, this.Setting.CertificatePassword));
            request.Method = "POST";
            request.Proxy = null;
            request.ContentType = "application/json; charset=utf-8";

            var message = CreatePostMessage(method, parametersJson, sign);
            var data = Encoding.UTF8.GetBytes(message);

            request.ContentLength = data.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(data, 0, data.Length);

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var responseStream = new StreamReader(stream))
                return responseStream.ReadToEnd();
        }

        private string CreatePostMessage(string method, string parametersJson, bool sign)
        {
            var json = new Dictionary<string, string>();
            Action<string, object> add = (key, value) => json[key] = "\"" + value + "\"";
            add("method", method);
            add("locale", typeof(YapiLanguage).GetField(this.Setting.Language.ToString()).GetCustomAttributes(typeof(JsonPropertyAttribute), false).Cast<JsonPropertyAttribute>().First().PropertyName);

            if (sign)
            {
                var signature = new YandexSignature(this.Setting.MasterToken, method, this.Setting.Login);
                add("finance_token", signature.Token);
                add("operation_num", signature.OperationId);
                add("login", signature.Login);
            }

            if (parametersJson != null)
                if (parametersJson.StartsWith("[") || parametersJson.StartsWith("{"))
                    json["param"] = parametersJson;
                else
                    json["param"] = "\"" + parametersJson + "\"";

            var merged = json
                .Where(x => x.Value != null)
                .Select(x => string.Format("\"{0}\": {1}", x.Key, x.Value))
                .Merge(", ");

            return string.Format("{{{0}}}", merged);
        }

        private T Request<T>(string method, object requestData = null, bool sign = false)
        {
            var json = HttpRequest(method, requestData == null ? null : SerializeToJson(requestData), sign);
            var error = JsonConvert.DeserializeObject<YandexErrorInfo>(json, JsonSettings);
            if (error != null && error.Code != 0)
                throw new ApplicationException(error.Error);
            var response = JsonConvert.DeserializeObject<YapiResponse<T>>(json, JsonSettings);
            return response.Data;
        }

        private string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None, JsonSettings);
        }

        #endregion

        #region PingApi

        public int PingApi()
        {
            return Request<int>(ApiCommand.PingApi);
        }

        public bool TestApiConnection()
        {
            return PingApi() == 1;
        }

        #endregion

        #region GetClientLogins

        public List<ShortClientInfo> GetClientLogins()
        {
            return Request<List<ShortClientInfo>>(ApiCommand.GetClientsList);
        }

        #endregion

        #region GetClientCampaigns

        public List<ShortCampaignInfo> GetClientCampaigns(params string[] logins)
        {
            return Request<List<ShortCampaignInfo>>(ApiCommand.GetCampaignsList, logins);
        }

        #endregion

        #region TransferMoney

        public void TransferMoney(TransferInfo from, TransferInfo to)
        {
            TransferMoney(new[] { from }, new[] { to });
        }

        public void TransferMoney(TransferInfo[] from, TransferInfo[] to)
        {
            var request = new { FromCampaigns = from, ToCampaigns = to };
            Request<int>(ApiCommand.TransferMoney, request, true);
        }

        #endregion

        public List<BannerInfo> GetBanners(int campId)
        {
            var request = new BannerRequestInfo { CampaignId = new[] { campId } };
            return Request<List<BannerInfo>>(ApiCommand.GetBanners, request);
        }

        public int[] CreateOrUpdateBanners(params BannerInfo[] banners)
        {
            Contract.Requires(banners != null && banners.Any());
            return Request<int[]>(ApiCommand.CreateOrUpdateBanners, banners);
        }

        public ClientUnitInfo[] GetClientsUnits(params string[] logins)
        {
            Contract.Requires(logins != null && logins.Any());
            return Request<ClientUnitInfo[]>(ApiCommand.GetClientsUnits, logins);
        }
    }
}
