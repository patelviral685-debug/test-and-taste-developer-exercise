using System;

namespace Test_Taste_Console_Application.Constants
{
    public static class HttpClientSettings
    {
        public const string JsonType = "application/json";

        /// <summary>
        /// Optional Bearer API key for api.le-systeme-solaire.net.
        /// Can be provided via the SOLAR_SYSTEM_API_KEY environment variable
        /// or set directly for authenticated API queries.
        /// </summary>
        public static string ApiKey => Environment.GetEnvironmentVariable("SOLAR_SYSTEM_API_KEY");
    }
}
