using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Test_Taste_Console_Application.Constants;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <summary>
    /// A service to configure and provide the HttpClient.
    /// Supports Bearer token authentication for api.le-systeme-solaire.net.
    /// </summary>
    public class HttpClientService
    {
        public HttpClient Client { get; }

        public HttpClientService(HttpClient client)
        {
            Client = client;
            Client.BaseAddress = new Uri(UriPath.BaseUri);
            Client.DefaultRequestHeaders.Accept.Add(new
                MediaTypeWithQualityHeaderValue(HttpClientSettings.JsonType));

            // Attach Bearer token if an API key is configured
            if (!string.IsNullOrWhiteSpace(HttpClientSettings.ApiKey))
            {
                Client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", HttpClientSettings.ApiKey);
            }
        }
    }
}
