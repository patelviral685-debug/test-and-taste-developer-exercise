namespace Test_Taste_Console_Application.Domain.Objects
{
    /// <summary>
    /// Represents static sky position data for celestial bodies as seen by an observer on Earth,
    /// as specified in moon-current-temperature-static-data-logic.md.
    /// </summary>
    public sealed class SkyPosition
    {
        public string Name { get; set; } = string.Empty;
        public string RightAscension { get; set; } = string.Empty;
        public string Declination { get; set; } = string.Empty;
        public string Azimuth { get; set; } = string.Empty;
        public string Altitude { get; set; } = string.Empty;
    }
}
