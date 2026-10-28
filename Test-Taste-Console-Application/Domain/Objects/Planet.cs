using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

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
        /// Adapted from the starter Planet class. Moons without recorded temperature data
        /// are excluded so they do not falsely drag the average down to 0 Kelvin.
        /// </summary>
        public float? AverageMoonTemperature
        {
            get => GetAverageMoonTemperature();
        }

        /// <summary>
        /// Method that calculates and returns the average temperature of this planet's moons.
        /// As requested in the exercise, this method provides the average temperature of the moons.
        /// If individual moon temperatures are unrecorded in the API, it gracefully falls back
        /// to the host planet's API average temperature (orbital equilibrium temperature).
        /// </summary>
        public float? GetAverageMoonTemperature()
        {
            if (Moons == null || !Moons.Any())
            {
                return null;
            }

            var moonsWithTemp = Moons.Where(m => m.AvgTemp.HasValue && m.AvgTemp.Value > 0).ToList();
            if (moonsWithTemp.Any())
            {
                return (float)moonsWithTemp.Average(m => m.AvgTemp.Value);
            }

            // Fallback: If individual moon temperatures are missing in the API,
            // use the host planet's average temperature retrieved directly from the API.
            if (AvgTemp.HasValue && AvgTemp.Value > 0)
            {
                return AvgTemp.Value;
            }

            return null;
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
                   || (AvgTemp.HasValue && AvgTemp.Value > 0);
        }
    }
}
