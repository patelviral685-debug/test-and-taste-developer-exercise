using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Test_Taste_Console_Application.Utilities
{
    /// <summary>
    /// Parses signed degree-minute-second (DMS) angle strings (e.g., "+4°54'32\"", "-11°56'05\"") into numeric decimal degrees.
    /// Implements the parser specified in moon-current-temperature-static-data-logic.md.
    /// </summary>
    public static class AngleParser
    {
        private static readonly Regex DmsPattern = new Regex(
            @"^\s*(?<sign>[+-])?\s*(?<deg>\d+)\s*°\s*(?<min>\d+)\s*['′]\s*(?<sec>\d+(?:\.\d+)?)\s*[""″]?\s*$",
            RegexOptions.Compiled);

        public static double ParseDegrees(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Angle value cannot be null or empty.", nameof(value));
            }

            var match = DmsPattern.Match(value);

            if (!match.Success)
            {
                throw new FormatException($"Invalid angle: '{value}'");
            }

            double degrees = double.Parse(
                match.Groups["deg"].Value, CultureInfo.InvariantCulture);
            double minutes = double.Parse(
                match.Groups["min"].Value, CultureInfo.InvariantCulture);
            double seconds = double.Parse(
                match.Groups["sec"].Value, CultureInfo.InvariantCulture);

            double result = degrees + (minutes / 60.0) + (seconds / 3600.0);

            if (match.Groups["sign"].Value == "-")
            {
                result = -result;
            }

            return result;
        }
    }
}
