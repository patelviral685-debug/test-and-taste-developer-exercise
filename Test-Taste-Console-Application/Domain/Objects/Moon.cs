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
        /// In the Solar System API, unrecorded or missing temperatures are represented as null or 0.
        /// Since 0 Kelvin (absolute zero) is physically not possible for celestial bodies,
        /// any value &lt;= 0 is intentionally treated as null (missing data).
        /// </summary>
        public float? AvgTemp { get; set; }

        public Moon(MoonDto moonDto)
        {
            Id = moonDto.Id;
            MassValue = moonDto.MassValue;
            MassExponent = moonDto.MassExponent;
            Gravity = moonDto.Gravity;

            // Treat <= 0 or null as unrecorded/missing temperature
            AvgTemp = (moonDto.AvgTemp.HasValue && moonDto.AvgTemp.Value > 0)
                ? moonDto.AvgTemp.Value
                : (float?)null;
        }
    }
}
