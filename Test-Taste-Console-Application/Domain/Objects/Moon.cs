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
        /// Average surface temperature in Kelvin.
        /// If the API provides a valid positive temperature, it is used directly.
        /// When the API returns 0 or null (unmeasured in the open dataset),
        /// it falls back to known astronomical reference temperatures where available.
        /// If still unavailable, it remains null rather than 0 Kelvin (absolute zero).
        /// </summary>
        public float? AvgTemp { get; set; }

        // Standard scientific reference surface temperatures (in Kelvin) for major moons
        // used when the public API returns 0 (unrecorded in their database)
        private static readonly Dictionary<string, float> KnownMoonTemperatures = new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase)
        {
            { "lune", 250f },
            { "moon", 250f },
            { "phobos", 233f },
            { "deimos", 233f },
            { "io", 130f },
            { "europe", 102f },
            { "ganymede", 110f },
            { "callisto", 134f },
            { "mimas", 64f },
            { "encelade", 75f },
            { "tethys", 86f },
            { "dione", 87f },
            { "rhea", 73f },
            { "titan", 94f },
            { "iapet", 130f },
            { "ariel", 60f },
            { "umbriel", 75f },
            { "titania", 60f },
            { "oberon", 61f },
            { "miranda", 60f },
            { "triton", 38f },
            { "nereide", 50f },
            { "protee", 50f }
        };

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
            // If the API returns 0 or null, check if a scientific reference temperature exists
            else if (!string.IsNullOrEmpty(Id) && KnownMoonTemperatures.TryGetValue(Id.ToLowerInvariant().Trim(), out var refTemp))
            {
                AvgTemp = refTemp;
            }
            else
            {
                AvgTemp = null;
            }
        }
    }
}
