using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FlightPlan.Service
{
    public class HttpService
    {
        public string BaseUrl { get; private set; }

        private const string URL_PART = "api";

        public HttpService(string baseUrl)
        {
            BaseUrl = baseUrl;
        }

        public string ServiceURL
        {
            get
            {
                if (BaseUrl.EndsWith("/"))
                    return BaseUrl + URL_PART + "/{0}";
                else
                    return BaseUrl + "/" + URL_PART + "/{0}";
            }
        }

        public async Task<T> Post<T>(string url, string serialized)
        {
            return await Post<T>(url, serialized, "application/json");
        }

        public async Task<T> Post<T>(string url, string serialized, string contentType)
        {
            return await Post<T>(url, serialized, contentType, string.Empty);
        }

        public async Task<T> Post<T>(string url, string serialized, string contentType, string authorizationToken)
        {
            string content = await Post(url, serialized, contentType, authorizationToken);
            var obj = JsonConvert.DeserializeObject<T>(content);
            return obj;
        }

        public async Task<string> Post(string url, string serialized, string contentType, string authorizationToken)
        {
            string content = "";

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (!String.IsNullOrEmpty(authorizationToken))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);

                var response = await httpClient.PostAsync(new Uri(url), new StringContent(serialized, UnicodeEncoding.UTF8, contentType));
                content = await response.Content.ReadAsStringAsync();
            }

            return content;
        }

        public async Task<T> Get<T>(string url)
        {
            return await Get<T>(url, "application/json");
        }

        public async Task<T> Get<T>(string url, string contentType)
        {
            return await Get<T>(url, contentType, string.Empty);
        }

        public async Task<T> Get<T>(string url, string contentType, string authorizationToken)
        {
            string content = await Get(url, contentType, authorizationToken);
            var obj = JsonConvert.DeserializeObject<T>(content);
            return obj;
        }

        public async Task<string> Get(string url, string contentType, string authorizationToken)
        {
            string content = "";
            var requestTimeout = TimeSpan.FromSeconds(300);
            var httpTimeout = TimeSpan.FromSeconds(300);

            using (var httpClient = new System.Net.Http.HttpClient())
            {
                httpClient.Timeout = httpTimeout;
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

                if (!String.IsNullOrEmpty(authorizationToken))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationToken);
                
                using (var tokenSource = new CancellationTokenSource(requestTimeout))
                {
                    var response = await httpClient.GetAsync(new Uri(url), tokenSource.Token);
                    content = await response.Content.ReadAsStringAsync();
                }
            }
            return content;
        }

        public string GetFullUrl(string method)
        {
            var url = String.Format(ServiceURL, method);
            return url;
        }

        public Dictionary<string, string> ParseQueryString(Uri uri)
        {
            var query = uri.Query.Substring(uri.Query.IndexOf('?') + 1); // +1 for skipping '?'
            var pairs = query.Split('&');
            return pairs
                .Select(o => o.Split('='))
                .Where(items => items.Count() == 2)
                .ToDictionary(pair => Uri.UnescapeDataString(pair[0]),
                    pair => Uri.UnescapeDataString(pair[1]));
        }
    }
}
