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
        public ICollection<Moon> Moons { get; set; }

        /// <summary>
        /// Calculates the average gravity of this planet's moons.
        /// Moons without gravity data (0 or negative) are excluded.
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
        /// Calculates the average temperature (in Kelvin) of this planet's moons.
        /// Only moons with valid recorded temperature data (AvgTemp != null and > 0)
        /// are included in the average. If no moons have recorded temperature data,
        /// returns null to avoid misrepresenting missing data as absolute zero.
        /// </summary>
        public float? AverageMoonTemperature
        {
            get
            {
                if (Moons == null || !Moons.Any())
                {
                    return null;
                }

                var moonsWithTemp = Moons.Where(m => m.AvgTemp.HasValue && m.AvgTemp.Value > 0).ToList();
                return moonsWithTemp.Any() ? (float)moonsWithTemp.Average(m => m.AvgTemp.Value) : (float?)null;
            }
        }

        public Planet(PlanetDto planetDto)
        {
            Id = planetDto.Id;
            SemiMajorAxis = planetDto.SemiMajorAxis;
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
            return Moons != null && Moons.Any(m => m.AvgTemp.HasValue && m.AvgTemp.Value > 0);
        }
    }
}
