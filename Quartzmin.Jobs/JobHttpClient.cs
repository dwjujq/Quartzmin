using Quartz;
using Quartz.Impl.AdoJobStore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Quartzmin.Jobs
{
    public class JobHttpClient
    {
        private HttpClient _httpClient;

        private IScheduler _scheduler;

        private string _url;

        public JobHttpClient(string name, IScheduler scheduler)
        { 
            _scheduler= scheduler;
            var httpClientFactory=_scheduler.Context.GetHttpClientFactory();
            _httpClient=httpClientFactory.CreateClient(name);
        }

        public static JobHttpClient Create(string name, IScheduler scheduler)
        {
            return new JobHttpClient(name, scheduler);
        }

        public JobHttpClient WithUrl(string url)
        {
            _url = url;
            return this;
        }

        public JobHttpClient AddHeader(string key, string value)
        {
            _httpClient.DefaultRequestHeaders.Add(key, value);
            return this;
        }

        public JobHttpClient WithHeaders(IDictionary<string,string> headers)
        {
            foreach(var header in headers)
            {
                _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            return this;
        }

        public JobHttpClient WithQueryParams(string queryParams)
        {
            if (!_url.EndsWith("?"))
            {
                _url += "?";
            }
            _url += queryParams;
            return this;
        }

        public JobHttpClient AddQueryParam(string key, string value)
        {
            if(!_url.EndsWith("?"))
            {
                _url += "?";
            }
            _url += $"{key}={value}";
            return this;
        }

        public JobHttpClient WithQueryParam(IDictionary<string,string> queryDic)
        {
            if (!_url.EndsWith("?"))
            {
                _url += "?";
            }
            var sb = new StringBuilder();
            foreach(var pair in queryDic)
            {
                sb.AppendFormat("{0}={1}&", pair.Key, pair.Value);
            }
            if(sb.Length>0)
            {
                sb = sb.Remove(sb.Length - 1, 1);
                _url+= sb.ToString();
            }
            return this;
        }

        public async Task<T> GetAsync<T>(JsonSerializerOptions serializerOptions = null, CancellationToken cancellationToken=default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _url);

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>(serializerOptions, cancellationToken).ConfigureAwait(false);
        }        
    }
}
