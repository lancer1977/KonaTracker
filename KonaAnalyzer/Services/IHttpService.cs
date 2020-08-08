using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace KonaAnalyzer.Services
{
    public interface IHttpService
    {
        HttpClient CreateHttp();


    }

    public class HttpService : IHttpService
    { 
        public static IHttpService Instance { get; set; }
        public static Func<HttpMessageHandler> GetHandler { get; set; }
        public HttpClient CreateHttp()
        {
 
            var httpClient = GetHandler == null ? new HttpClient() : new HttpClient(GetHandler());
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/json"));
            return httpClient;
        }


    }

}