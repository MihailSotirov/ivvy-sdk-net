using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ivvy.API.Json.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ivvy.API
{
    /// <summary>
    /// The primary class used to call methods of the iVvy api.
    /// </summary>
    public partial class ApiClient : IApiClient
    {
        private readonly Dictionary<string, ApiCallDetails> apiCallDetailsStorage =
            new Dictionary<string, ApiCallDetails>();

        /// <summary>
        /// The api version.
        /// </summary>
        public string ApiVersion
        {
            get; set;
        }

        /// <summary>
        /// The base url of the api.
        /// </summary>
        public string BaseUrl
        {
            get; set;
        }

        /// <summary>
        /// The key required to call api methods.
        /// </summary>
        public string ApiKey
        {
            get; set;
        }

        /// <summary>
        /// The secret required to call api methods.
        /// </summary>
        public string ApiSecret
        {
            get; set;
        }

        /// <summary>
        /// Http client used to call the iVvy api.
        /// </summary>
        private static readonly HttpClient httpClient = new HttpClient();

        /// <summary>
        /// This error code is used to represent an error related to this library implementation.
        /// </summary>
        private static readonly string libErrorCode = "000";

        public ApiClient()
        {
            ApiVersion = "1.0";
        }

        /// <summary>
        /// Calls a method of the iVvy api. Encapsulates the header and
        /// signing requirements.
        /// </summary>
        public async Task<ResultOrError<T>> CallAsync<T>(
            string apiNamespace, string action, object requestData) where T : new()
        {
            if (ApiVersion != "1.0")
            {
                throw new ArgumentException("ApiVersion is not a valid version number");
            }
            var postData = "";
            if (requestData != null)
            {
                postData = JsonConvert.SerializeObject(
                    requestData,
                    Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = new List<JsonConverter>() { new IsoDateTimeConverter() { DateTimeFormat = Utils.DateTimeFormat } }
                    }
                );
            }
            var method = "POST";
            var contentType = "application/json; charset=utf-8";
            var contentMD5 = Utils.GetMD5Hash(postData);
            var date = "";
            var requestUri = "/api/" + ApiVersion + "/" + apiNamespace + "?action=" + action;
            var ivvyDate = DateTime.Now.ToUniversalTime().ToString(Utils.DateTimeFormat);
            var headersToSign = "IVVYDate=" + ivvyDate;
            var initialStringToSign = method + contentMD5 + contentType + date + requestUri + ApiVersion + headersToSign;
            var stringToSign = initialStringToSign.ToLower();
            var signature = Utils.SignString(stringToSign, ApiSecret);

            var message = new HttpRequestMessage(HttpMethod.Post, BaseUrl + requestUri);
            message.Headers.Clear();
            message.Content = new StringContent(postData, Encoding.UTF8, "application/json");
            message.Content.Headers.Add("Content-MD5", contentMD5);
            message.Content.Headers.Add("IVVY-Date", ivvyDate);
            message.Content.Headers.Add("X-Api-Authorization", "IWS " + ApiKey + ":" + signature);

            string detailsKey = null;
            HttpResponseMessage httpResponse = null;
            ResultOrError<T> result = null;
            try
            {
                httpResponse = await httpClient.SendAsync(message);
                var data = await httpResponse.Content.ReadAsStringAsync();
                // Set the call details before the deserialization to avoid missing it in case of deserialization error.
                detailsKey = InsertCallDetails(
                    httpResponse.StatusCode,
                    postData,
                    data,
                    message.Headers,
                    httpResponse.Headers);

                result = JsonConvert.DeserializeObject<ResultOrError<T>>(data, new ResponseConverter<T>());
                if (result == null)
                {
                    result = new ResultOrError<T>()
                    {
                        ErrorCode = libErrorCode,
                        ErrorCodeSpecific = "CallAsync",
                        ErrorMessage = "Received invalid response.",
                    };
                }
            }
            catch (Exception ex)
            {
                result = new ResultOrError<T>()
                {
                    ErrorCode = libErrorCode,
                    ErrorCodeSpecific = "CallAsync",
                    ErrorMessage = ex.Message,
                };
            }
            finally
            {
                if (httpResponse != null)
                {
                    httpResponse.Dispose();
                }
            }

            result.ApiCallDetailsKey = detailsKey;

            return result;
        }

        public ApiCallDetails GetApiCallDetails(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return default;
            }

            if (!apiCallDetailsStorage.ContainsKey(key))
            {
                return default;
            }

            var details = apiCallDetailsStorage[key];
            // Remove it in order to be fetchable only once per instance.
            apiCallDetailsStorage.Remove(key);

            return details;
        }

        /// <summary>
        /// Inserts the details into a storage and returns the key.
        /// </summary>
        /// <returns></returns>
        private string InsertCallDetails(
            HttpStatusCode statusCode,
            string requestBody,
            string responseBody,
            HttpHeaders requestHeaders,
            HttpHeaders responseHeaders)
        {
            var details = new ApiCallDetails
            {
                StatusCode = statusCode,
                RequestBody = requestBody,
                ResponseBody = responseBody,
                RequestHeaders = requestHeaders,
                ResponseHeaders = responseHeaders
            };
            var key = Guid.NewGuid().ToString("N");
            apiCallDetailsStorage[key] = details;

            return key;
        }
    }
}