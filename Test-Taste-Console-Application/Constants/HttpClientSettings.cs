using System;

namespace Test_Taste_Console_Application.Constants
{
    public static class HttpClientSettings
    {
        public const string JsonType = "application/json";

        /// <summary>
        /// Bearer API key for api.le-systeme-solaire.net.
        /// Can be overridden via the SOLAR_SYSTEM_API_KEY environment variable.
        /// </summary>
        public const string DefaultApiKey = "3ee6d3b5-9a79-435c-9769-ea5b39d255a8";

        public static string ApiKey =>
            Environment.GetEnvironmentVariable("SOLAR_SYSTEM_API_KEY") ?? DefaultApiKey;
    }
}
