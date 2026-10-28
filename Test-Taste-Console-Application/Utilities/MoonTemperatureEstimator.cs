using System;

namespace Test_Taste_Console_Application.Utilities
{
    /// <summary>
    /// Implements the proxy temperature estimation logic using static observer Sun altitude data
    /// as specified in moon-current-temperature-static-data-logic.md.
    /// </summary>
    public static class MoonTemperatureEstimator
    {
        private const double SolarFluxAtOneAu = 1361.0; // W/m²
        private const double StefanBoltzmannConstant = 5.670374419e-8;
        private const double DefaultAlbedo = 0.12;
        private const double DefaultEmissivity = 0.95;

        /// <summary>
        /// Estimates the illustrative proxy temperature in Celsius from the Sun's altitude.
        /// Note: This is an observer-based proxy demonstrating static data flow, not a direct measured lunar temperature.
        /// </summary>
        public static double EstimateProxyCelsiusFromSunAltitude(
            double sunAltitudeDegrees,
            double albedo = DefaultAlbedo,
            double emissivity = DefaultEmissivity)
        {
            if (albedo < 0 || albedo > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(albedo), "Albedo must be between 0 and 1.");
            }

            if (emissivity <= 0 || emissivity > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(emissivity), "Emissivity must be greater than 0 and at most 1.");
            }

            // DEMO ONLY: this is an observer-based altitude, not the
            // incidence angle at a specific location on the Moon.
            double incidenceProxyRadians = sunAltitudeDegrees * Math.PI / 180.0;
            double illuminationProxy = Math.Max(0.0, Math.Cos(incidenceProxyRadians));

            if (illuminationProxy == 0)
            {
                throw new InvalidOperationException(
                    "The Sun is below the observer's horizon. " +
                    "This proxy cannot estimate lunar surface temperature.");
            }

            double absorbedFlux = SolarFluxAtOneAu * (1.0 - albedo) * illuminationProxy;

            double temperatureKelvin = Math.Pow(
                absorbedFlux / (emissivity * StefanBoltzmannConstant),
                0.25);

            return temperatureKelvin - 273.15;
        }

        /// <summary>
        /// Estimates the illustrative proxy temperature in Kelvin from the Sun's altitude.
        /// </summary>
        public static double EstimateProxyKelvinFromSunAltitude(
            double sunAltitudeDegrees,
            double albedo = DefaultAlbedo,
            double emissivity = DefaultEmissivity)
        {
            return EstimateProxyCelsiusFromSunAltitude(sunAltitudeDegrees, albedo, emissivity) + 273.15;
        }
    }
}
