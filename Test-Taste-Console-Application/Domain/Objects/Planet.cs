using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Test_Taste_Console_Application.Domain.Data;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class Planet
    {
        public string Id { get; set; }
        public float SemiMajorAxis { get; set; }

        /// <summary>
        /// Average temperature (in Kelvin) of the planet itself, retrieved directly from the Solar System API.
        /// </summary>
        public float? AvgTemp { get; set; }

        public ICollection<Moon> Moons { get; set; }

        /// <summary>
        /// Calculates the average gravity of this planet's moons.
        /// Moons without positive gravity data are excluded.
        /// </summary>
        public float AverageMoonGravity
        {
            get
            {
                if (Moons == null || !Moons.Any())
                {
                    return 0.0f;
                }

                var moonsWithGravity = Moons.Where(m => m.Gravity > 0).ToList();
                return moonsWithGravity.Any() ? (float)moonsWithGravity.Average(m => m.Gravity) : 0.0f;
            }
        }

        /// <summary>
        /// Property that provides the average temperature (in Kelvin) of this planet's moons.
        /// Adapted from the starter Planet class.
        /// </summary>
        public float? AverageMoonTemperature
        {
            get => GetAverageMoonTemperature();
        }

        // Standard Bond albedo values for major solar system moons used in radiative equilibrium calculation
        private static readonly Dictionary<string, double> KnownMoonAlbedos = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase)
        {
            { "lune", 0.12 },
            { "moon", 0.12 },
            { "phobos", 0.071 },
            { "deimos", 0.068 },
            { "io", 0.63 },
            { "europe", 0.67 },
            { "ganymede", 0.43 },
            { "callisto", 0.22 },
            { "mimas", 0.96 },
            { "encelade", 0.81 },
            { "tethys", 0.80 },
            { "dione", 0.70 },
            { "rhea", 0.70 },
            { "titan", 0.22 },
            { "iapet", 0.15 },
            { "ariel", 0.39 },
            { "umbriel", 0.21 },
            { "titania", 0.27 },
            { "oberon", 0.23 },
            { "miranda", 0.32 },
            { "triton", 0.80 },
            { "nereide", 0.14 },
            { "protee", 0.096 }
        };

        /// <summary>
        /// Method that calculates and returns the average temperature of this planet's moons (in Kelvin).
        /// 1. If moons possess recorded positive temperatures from the API, their mean is calculated.
        /// 2. If moon temperature is unrecorded (0/null in API), calculates the radiative equilibrium
        ///    temperature using this planet's SemiMajorAxis (distance from Sun in km) and the moon's
        ///    Bond albedo and thermal emissivity (via MoonTemperatureCalculator).
        /// 3. If orbital distance is unavailable, gracefully falls back to the host planet's API average temperature.
        /// </summary>
        public float? GetAverageMoonTemperature()
        {
            if (Moons == null || !Moons.Any())
            {
                return null;
            }

            var temperatures = new List<float>();

            foreach (var moon in Moons)
            {
                // 1. If moon has valid positive temperature from API, use it
                if (moon.AvgTemp.HasValue && moon.AvgTemp.Value > 0)
                {
                    temperatures.Add(moon.AvgTemp.Value);
                }
                // 2. Otherwise calculate radiative equilibrium temperature using the formula from moon-average-temperature-implementation.md
                else if (SemiMajorAxis > 0)
                {
                    double albedo = 0.30; // default planetary satellite albedo
                    if (!string.IsNullOrEmpty(moon.Id) && KnownMoonAlbedos.TryGetValue(moon.Id.ToLowerInvariant().Trim(), out var knownAlbedo))
                    {
                        albedo = knownAlbedo;
                    }

                    double emissivity = 0.90; // standard thermal emissivity
                    double eqTempK = CalculateMoonEquilibriumTemperatureKelvin(SemiMajorAxis, albedo, emissivity);
                    temperatures.Add((float)eqTempK);
                }
            }

            if (temperatures.Any())
            {
                return (float)temperatures.Average();
            }

            // Fallback: If no temperatures could be calculated, use host planet's API avgTemp
            if (AvgTemp.HasValue && AvgTemp.Value > 0)
            {
                return AvgTemp.Value;
            }

            return null;
        }

        /// <summary>
        /// Calculates a moon's radiative equilibrium temperature in Celsius using the parent planet's orbital distance
        /// from the Sun (in kilometers) and the moon's assumed Bond albedo and thermal emissivity.
        /// Delegates to MoonTemperatureCalculator.
        /// </summary>
        public static double CalculateMoonEquilibriumTemperatureCelsius(
            double planetDistanceFromSunKm,
            double moonBondAlbedo = 0.12,
            double moonThermalEmissivity = 0.90)
        {
            return MoonTemperatureCalculator.CalculateEquilibriumTemperatureCelsius(
                planetDistanceFromSunKm,
                moonBondAlbedo,
                moonThermalEmissivity);
        }

        /// <summary>
        /// Calculates a moon's radiative equilibrium temperature in Kelvin using the parent planet's orbital distance
        /// from the Sun (in kilometers) and the moon's assumed Bond albedo and thermal emissivity.
        /// Delegates to MoonTemperatureCalculator.
        /// </summary>
        public static double CalculateMoonEquilibriumTemperatureKelvin(
            double planetDistanceFromSunKm,
            double moonBondAlbedo = 0.12,
            double moonThermalEmissivity = 0.90)
        {
            return MoonTemperatureCalculator.CalculateEquilibriumTemperatureKelvin(
                planetDistanceFromSunKm,
                moonBondAlbedo,
                moonThermalEmissivity);
        }

        /// <summary>
        /// Estimates the illustrative proxy temperature (in Celsius) for the Moon using static Sun altitude data
        /// as specified in moon-current-temperature-static-data-logic.md.
        /// </summary>
        public static double EstimateMoonCurrentTemperatureProxyCelsius(double albedo = 0.12, double emissivity = 0.95)
        {
            double sunAltitude = AngleParser.ParseDegrees(StaticAstronomicalData.Sun.Altitude);
            return MoonTemperatureEstimator.EstimateProxyCelsiusFromSunAltitude(sunAltitude, albedo, emissivity);
        }

        /// <summary>
        /// Estimates the illustrative proxy temperature (in Kelvin) for the Moon using static Sun altitude data
        /// as specified in moon-current-temperature-static-data-logic.md.
        /// </summary>
        public static double EstimateMoonCurrentTemperatureProxyKelvin(double albedo = 0.12, double emissivity = 0.95)
        {
            return EstimateMoonCurrentTemperatureProxyCelsius(albedo, emissivity) + 273.15;
        }

        public Planet(PlanetDto planetDto)
        {
            Id = planetDto.Id;
            SemiMajorAxis = planetDto.SemiMajorAxis;
            AvgTemp = (planetDto.AvgTemp.HasValue && planetDto.AvgTemp.Value > 0) ? planetDto.AvgTemp : null;
            Moons = new Collection<Moon>();
            if (planetDto.Moons != null)
            {
                foreach (MoonDto moonDto in planetDto.Moons)
                {
                    Moons.Add(new Moon(moonDto));
                }
            }
        }

        public bool HasMoons()
        {
            return Moons != null && Moons.Count > 0;
        }

        public bool HasMoonTemperatures()
        {
            return (Moons != null && Moons.Any(m => m.AvgTemp.HasValue && m.AvgTemp.Value > 0))
                   || (AvgTemp.HasValue && AvgTemp.Value > 0)
                   || (SemiMajorAxis > 0);
        }
    }
}
