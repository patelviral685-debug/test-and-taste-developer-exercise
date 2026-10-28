using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Newtonsoft.Json;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Data;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.DataTransferObjects.JsonObjects;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class MoonService : IMoonService
    {
        private readonly HttpClientService _httpClientService;

        public MoonService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Moon> GetAllMoons()
        {
            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllMoonsWithMassQueryParameters)
                    .Result;

                // Handle HTTP errors gracefully
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Logger.Instance.Warn(
                            $"{LoggerMessage.GetRequestFailed}{response.StatusCode}. " +
                            "The Solar System OpenData API requires an API key (Bearer token). " +
                            "Set the SOLAR_SYSTEM_API_KEY environment variable. Using sample moon dataset.");
                    }
                    else
                    {
                        Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    }

                    // Fallback to reference Solar System dataset
                    return SolarSystemSampleData.GetSampleMoons();
                }

                var content = response.Content.ReadAsStringAsync().Result;

                var allMoons = new Collection<Moon>();
                var results = JsonConvert.DeserializeObject<JsonResult<MoonDto>>(content);

                if (results == null || results.Bodies == null)
                {
                    return allMoons;
                }

                foreach (MoonDto moonDto in results.Bodies)
                {
                    allMoons.Add(new Moon(moonDto));
                }

                return allMoons;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error querying moons from API: {ex.Message}. Falling back to sample dataset.");
                return SolarSystemSampleData.GetSampleMoons();
            }
        }
    }
}
