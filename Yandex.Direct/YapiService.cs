﻿#region Usings
using System;
using System.Collections.Generic;
#if NET4
using System.Diagnostics.Contracts;
#endif
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
#endregion

namespace Yandex.Direct
{
    public partial class YapiService
    {
        #region Constructors and Properties

        public YapiSettings Setting { get; private set; }

        private JsonSerializerSettings JsonSettings { get; set; }

        public YapiService(YapiSettings settings)
        {
#if NET4
            Contract.Requires(settings != null);
#else
			if (settings == null) throw new ArgumentNullException("settings");
#endif
            this.Setting = settings;
            this.JsonSettings =
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
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

        #endregion

        #region Отправка запросов к API Яндекса

    	private string HttpRequest(string method, string parametersJson, bool sign)
        {
#if NET4
            Contract.Requires(!sign || !string.IsNullOrWhiteSpace(this.Setting.MasterToken), "Financial operations require MasterToken and Login to be set");
#else
			if(!(!sign || !string.IsNullOrEmpty(this.Setting.MasterToken))) throw new InvalidOperationException("Financial operations require MasterToken and Login to be set");
#endif
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
            Action<string, object> add = (key, value) => json[key] = value.ToString().StartsWith("\"") ? value.ToString() : "\"" + value + "\"";
            add("method", method);
            add("locale", JsonConvert.SerializeObject(this.Setting.Language, new StringEnumConverter()));

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

		private T Request<T>(string method)
		{
			return Request<T>(method, null);
		}

		private T Request<T>(string method, object requestData)
		{
			return Request<T>(method, requestData, false);
		}

        private T Request<T>(string method, object requestData, bool sign)
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

        #region Clients

        public List<ShortClientInfo> GetClientLogins()
        {
            return Request<List<ShortClientInfo>>(ApiCommand.GetClientsList);
        }

        public ClientUnitInfo[] GetClientsUnits(params string[] logins)
        {
#if NET4
            Contract.Requires(logins != null && logins.Any());
#endif
            return Request<ClientUnitInfo[]>(ApiCommand.GetClientsUnits, logins);
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

        #region Banners

        public List<BannerInfo> GetBanners(int campId)
        {
            var request = new BannerRequestInfo { CampaignId = new[] { campId } };
            return Request<List<BannerInfo>>(ApiCommand.GetBanners, request);
        }

        public int[] CreateOrUpdateBanners(params BannerInfo[] banners)
        {
#if NET4
            Contract.Requires(banners != null && banners.Any());
#endif
            return Request<int[]>(ApiCommand.CreateOrUpdateBanners, banners);
        }

        #endregion

        #region Reporting

        public int CreateReport(NewReportInfo reportInfo)
        {
#if NET4
            Contract.Requires(reportInfo != null && reportInfo.Limit.HasValue ^ reportInfo.Offset.HasValue);
#endif
            return Request<int>(ApiCommand.CreateNewReport, reportInfo);
        }

        public ReportInfo[] ListReports()
        {
            return Request<ReportInfo[]>(ApiCommand.GetReportList);
        }

        public GoalInfo[] GetStatGoals(int campId)
        {
            return Request<GoalInfo[]>(ApiCommand.GetStatGoals, new StatGoalRequestInfo(campId));
        }

        public void DeleteReport(int reportId)
        {
            var result = Request<int>(ApiCommand.DeleteReport, reportId);
            if (result!= 1)
                throw new ApplicationException("Плохой ответ. Должен вернуть: 1. Венул: " + result);
        }

        #endregion

		#region Prices
		public void UpdatePrices(PhrasePriceInfo[] prices)
		{
			var result = Request<int>(ApiCommand.UpdatePrices, prices);
			if (result != 1)
				throw new Exception("Error. ReturnCode <> 1; ReturnCode = " + result);
		}
		#endregion
    }
}
