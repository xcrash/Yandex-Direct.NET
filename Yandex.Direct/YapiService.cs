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
    public class YapiService
    {
        public static class ApiCommand
        {
            public const string TransferMoney = "TransferMoney";
            public const string PingApi = "PingAPI";
            public const string GetClientsList = "GetClientsList";
            public const string GetCampaignsList = "GetCampaignsList";
            public const string GetBanners = "GetBanners";
            public const string CreateOrUpdateBanners = "CreateOrUpdateBanners";
            public const string GetClientsUnits = "GetClientsUnits";
        }

        const string ApiAddress = @"https://soap.direct.yandex.ru/json-api/v4/";
        const string RequestLocale = "en";
        private string CertificatePath { get; set; }
        string CertificatePassword { get; set; }
        public string MasterToken { get; set; }
        public string DefaultLogin { get; set; }

        private JsonSerializerSettings JsonSettings { get; set; }


        public YapiService(string certificatePath, string certificatePassword)
        {
            this.CertificatePath = certificatePath;
            this.CertificatePassword = certificatePassword;

            this.JsonSettings =
                new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = { new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd" } }
                    };

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, errors) => true;
        }

        #region Отправка запросов к API Яндекса

        private string HttpRequest(string method, string parametersJson, bool sign = false)
        {
            var request = (HttpWebRequest)WebRequest.Create(ApiAddress);
            request.ClientCertificates.Add(new X509Certificate2(CertificatePath, CertificatePassword));
            request.Method = "POST";
            request.Proxy = null;
            request.ContentType = "application/json; charset=utf-8";

            var json = new Dictionary<string, string>();

            Action<string, object> add = (key, value) => json[key] = "\"" + value + "\"";

            add("method", method);
            add("locale", RequestLocale);

            if (sign)
            {
                var signature = new YandexSignature(method, this.DefaultLogin);
                add("finance_token", signature.Token);
                add("operation_num", signature.OperationId);
                add("login", signature.Login);
            }

            if (parametersJson != null)
                json["param"] = parametersJson.StartsWith("[") || parametersJson.StartsWith("{") ? parametersJson : ("\"" + parametersJson + "\"");

            var message = json
                .Where(x => x.Value != null)
                .Select(x => string.Format("\"{0}\": {1}", x.Key, x.Value))
                .Aggregate(new StringBuilder(), (sb, x) => sb.AppendFormat("{0}, ", x))
                .ToString()
                .TrimEnd(',', ' ');
            message = string.Format("{{{0}}}", message);

            var encoding = new UTF8Encoding();
            var data = encoding.GetBytes(message);
            string responseString;

            request.ContentLength = data.Length;
            using (var requestStream = request.GetRequestStream())
                requestStream.Write(data, 0, data.Length);

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var responseStream = new StreamReader(stream))
                responseString = responseStream.ReadToEnd();
            return responseString;
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
            return Request<int[]>(ApiCommand.CreateOrUpdateBanners, banners);
        }

        public ClientUnitInfo[] GetClientsUnits(params string[] logins)
        {
            Contract.Requires(logins!= null && logins.Any());
            return Request<ClientUnitInfo[]>(ApiCommand.GetClientsUnits, logins);
        }
    }
}
