using System;
using System.Collections.Generic;
using Test_Taste_Console_Application.Domain.DataTransferObjects;

namespace Test_Taste_Console_Application.Domain.Objects
{
    public class Moon
    {
        public string Id { get; set; }
        public float MassValue { get; set; }
        public float MassExponent { get; set; }
        public float Gravity { get; set; }

        /// <summary>
        /// Average surface temperature in Kelvin from the API.
        /// If the API provides a valid positive temperature, it is stored directly.
        /// When the API returns 0 or null (unmeasured in the open dataset), it is null,
        /// and Planet.cs calculates the moon's radiative equilibrium temperature.
        /// </summary>
        public float? AvgTemp { get; set; }

        private static readonly Dictionary<string, float> KnownMoonGravities = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase)
        {
            { "lune", 1.62f },
            { "moon", 1.62f },
            { "phobos", 0.0057f },
            { "deimos", 0.003f },
            { "io", 1.796f },
            { "europe", 1.315f },
            { "ganymede", 1.428f },
            { "callisto", 1.235f },
            { "mimas", 0.064f },
            { "encelade", 0.113f },
            { "tethys", 0.145f },
            { "dione", 0.231f },
            { "rhea", 0.264f },
            { "titan", 1.352f },
            { "iapet", 0.223f },
            { "ariel", 0.269f },
            { "umbriel", 0.2f },
            { "titania", 0.378f },
            { "oberon", 0.346f },
            { "miranda", 0.079f },
            { "triton", 0.779f },
            { "nereide", 0.071f },
            { "protee", 0.07f }
        };

        public Moon(MoonDto moonDto)
        {
            Id = moonDto.Id;
            MassValue = moonDto.MassValue;
            MassExponent = moonDto.MassExponent;
            Gravity = moonDto.Gravity;

            // Use API gravity if positive, otherwise fall back to reference gravity
            if (Gravity <= 0 && !string.IsNullOrEmpty(Id) && KnownMoonGravities.TryGetValue(Id.ToLowerInvariant().Trim(), out var refGrav))
            {
                Gravity = refGrav;
            }

            // If the API returns a valid positive temperature, use it
            if (moonDto.AvgTemp.HasValue && moonDto.AvgTemp.Value > 0)
            {
                AvgTemp = moonDto.AvgTemp.Value;
            }
            else
            {
                AvgTemp = null;
            }
        }
    }
}
