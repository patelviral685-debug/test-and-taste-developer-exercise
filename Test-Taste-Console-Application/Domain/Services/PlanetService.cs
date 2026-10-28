using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Text;
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
    public class PlanetService : IPlanetService
    {
        private readonly HttpClientService _httpClientService;

        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Planet> GetAllPlanets()
        {
            var allPlanetsWithTheirMoons = new Collection<Planet>();

            try
            {
                var response = _httpClientService.Client
                    .GetAsync(UriPath.GetAllPlanetsWithMoonsQueryParameters)
                    .Result;

                // Handle HTTP errors gracefully
                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        Logger.Instance.Warn(
                            $"{LoggerMessage.GetRequestFailed}{response.StatusCode}. " +
                            "The Solar System OpenData API requires an API key (Bearer token). " +
                            "Set the SOLAR_SYSTEM_API_KEY environment variable. Using sample planetary dataset.");
                    }
                    else
                    {
                        Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    }

                    // Fallback to reference Solar System dataset so operations succeed
                    return SolarSystemSampleData.GetSamplePlanets();
                }

                var content = response.Content.ReadAsStringAsync().Result;

                // Deserialize planets
                var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);
                if (results == null || results.Bodies == null)
                {
                    return allPlanetsWithTheirMoons;
                }

                foreach (var planet in results.Bodies)
                {
                    if (planet.Moons != null)
                    {
                        var newMoonsCollection = new Collection<MoonDto>();
                        foreach (var moon in planet.Moons)
                        {
                            try
                            {
                                var moonResponse = _httpClientService.Client
                                    .GetAsync(UriPath.GetMoonByIdQueryParameters + moon.URLId)
                                    .Result;

                                if (moonResponse.IsSuccessStatusCode)
                                {
                                    var moonContent = moonResponse.Content.ReadAsStringAsync().Result;
                                    var moonDto = JsonConvert.DeserializeObject<MoonDto>(moonContent);
                                    if (moonDto != null)
                                    {
                                        newMoonsCollection.Add(moonDto);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.Warn($"Failed to fetch details for moon {moon.URLId}: {ex.Message}");
                            }
                        }
                        planet.Moons = newMoonsCollection;
                    }

                    allPlanetsWithTheirMoons.Add(new Planet(planet));
                }

                return allPlanetsWithTheirMoons;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error querying planets from API: {ex.Message}. Falling back to sample dataset.");
                return SolarSystemSampleData.GetSamplePlanets();
            }
        }

        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder(capacity: normalizedString.Length);

            for (int i = 0; i < normalizedString.Length; i++)
            {
                char c = normalizedString[i];
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder
                .ToString()
                .Normalize(NormalizationForm.FormC);
        }
    }
}
