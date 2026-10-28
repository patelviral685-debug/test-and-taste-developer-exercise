using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
        private IEnumerable<Planet> _cachedPlanets;

        public PlanetService(HttpClientService httpClientService)
        {
            _httpClientService = httpClientService;
        }

        public IEnumerable<Planet> GetAllPlanets()
        {
            if (_cachedPlanets != null)
            {
                return _cachedPlanets;
            }

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
                            "Set the SOLAR_SYSTEM_API_KEY environment variable. Using reference planetary dataset.");
                    }
                    else
                    {
                        Logger.Instance.Warn($"{LoggerMessage.GetRequestFailed}{response.StatusCode}");
                    }

                    _cachedPlanets = SolarSystemSampleData.GetSamplePlanets();
                    return _cachedPlanets;
                }

                var content = response.Content.ReadAsStringAsync().Result;

                // Deserialize planets
                var results = JsonConvert.DeserializeObject<JsonResult<PlanetDto>>(content);
                if (results == null || results.Bodies == null)
                {
                    _cachedPlanets = allPlanetsWithTheirMoons;
                    return _cachedPlanets;
                }

                // Order planets by semi-major axis (distance from the Sun)
                var sortedBodies = results.Bodies.OrderBy(b => b.SemiMajorAxis).ToList();

                foreach (var planet in sortedBodies)
                {
                    if (planet.Moons != null && planet.Moons.Any())
                    {
                        var newMoonsCollection = new Collection<MoonDto>();

                        // Fetch individual moon details asynchronously in parallel to optimize network performance
                        var moonFetchTasks = planet.Moons.Select(async moon =>
                        {
                            try
                            {
                                var moonResponse = await _httpClientService.Client
                                    .GetAsync(UriPath.GetMoonByIdQueryParameters + moon.URLId)
                                    .ConfigureAwait(false);

                                if (moonResponse.IsSuccessStatusCode)
                                {
                                    var moonContent = await moonResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                                    return JsonConvert.DeserializeObject<MoonDto>(moonContent);
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Instance.Warn($"Failed to fetch details for moon {moon.URLId}: {ex.Message}");
                            }

                            return null;
                        }).ToList();

                        var fetchedMoons = Task.WhenAll(moonFetchTasks).Result;

                        foreach (var fetchedMoon in fetchedMoons)
                        {
                            if (fetchedMoon != null)
                            {
                                newMoonsCollection.Add(fetchedMoon);
                            }
                        }

                        planet.Moons = newMoonsCollection;
                    }

                    allPlanetsWithTheirMoons.Add(new Planet(planet));
                }

                _cachedPlanets = allPlanetsWithTheirMoons;
                return _cachedPlanets;
            }
            catch (Exception ex)
            {
                Logger.Instance.Error($"Error querying planets from API: {ex.Message}. Falling back to reference dataset.");
                _cachedPlanets = SolarSystemSampleData.GetSamplePlanets();
                return _cachedPlanets;
            }
        }

    }
}
