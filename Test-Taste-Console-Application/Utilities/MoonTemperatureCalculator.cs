using System;

namespace Test_Taste_Console_Application.Utilities
{
    /// <summary>
    /// Utility for estimating moon radiative equilibrium temperatures using the parent planet's
    /// orbital distance from the Sun (in kilometers) and the moon's Bond albedo and thermal emissivity.
    /// Implements the formula specified in moon-average-temperature-implementation.md.
    /// </summary>
    public static class MoonTemperatureCalculator
    {
        private const double SolarConstant = 1361.0;
        private const double AstronomicalUnitKm = 149597870.7;
        private const double StefanBoltzmannConstant = 5.670374419e-8;

        /// <summary>
        /// Calculates the idealized solar equilibrium temperature in Celsius.
        /// </summary>
        public static double CalculateEquilibriumTemperatureCelsius(
            double planetDistanceFromSunKm,
            double moonAlbedo = 0.12,
            double emissivity = 0.90)
        {
            double temperatureKelvin = CalculateEquilibriumTemperatureKelvin(
                planetDistanceFromSunKm,
                moonAlbedo,
                emissivity);

            return temperatureKelvin - 273.15;
        }

        /// <summary>
        /// Calculates the idealized solar equilibrium temperature in Kelvin.
        /// </summary>
        public static double CalculateEquilibriumTemperatureKelvin(
            double planetDistanceFromSunKm,
            double moonAlbedo = 0.12,
            double emissivity = 0.90)
        {
            if (planetDistanceFromSunKm <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(planetDistanceFromSunKm),
                    "Distance from the Sun must be greater than zero.");
            }

            if (moonAlbedo < 0 || moonAlbedo > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(moonAlbedo),
                    "Albedo must be between 0 and 1.");
            }

            if (emissivity <= 0 || emissivity > 1)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(emissivity),
                    "Emissivity must be greater than 0 and no greater than 1.");
            }

            double distanceAu = planetDistanceFromSunKm / AstronomicalUnitKm;

            double temperatureKelvin = Math.Pow(
                (SolarConstant * (1.0 - moonAlbedo)) /
                (4.0 * emissivity * StefanBoltzmannConstant *
                 Math.Pow(distanceAu, 2)),
                0.25);

            return temperatureKelvin;
        }
    }
}
