using System.Collections.Generic;
using Test_Taste_Console_Application.Domain.Objects;

namespace Test_Taste_Console_Application.Domain.Data
{
    /// <summary>
    /// Contains the static sky position data for the Sun, Moon, and planets
    /// as specified in moon-current-temperature-static-data-logic.md.
    /// </summary>
    public static class StaticAstronomicalData
    {
        public static readonly SkyPosition Sun = new SkyPosition
        {
            Name = "Soleil",
            RightAscension = "14h 11m 51s",
            Declination = "-13°16'16\"",
            Azimuth = "245°14'59\"",
            Altitude = "+4°54'32\""
        };

        public static readonly SkyPosition Moon = new SkyPosition
        {
            Name = "Lune",
            RightAscension = "4h 28m 18s",
            Declination = "+25°48'34\"",
            Azimuth = "31°12'09\"",
            Altitude = "-11°56'05\""
        };

        public static readonly IReadOnlyList<SkyPosition> AllPositions = new List<SkyPosition>
        {
            Sun,
            Moon,
            new SkyPosition { Name = "Mercury", RightAscension = "15h 05m 53s", Declination = "-19°55'38\"", Azimuth = "230°52'27\"", Altitude = "+7°38'34\"" },
            new SkyPosition { Name = "Venus", RightAscension = "13h 35m 46s", Declination = "-16°02'02\"", Azimuth = "249°55'07\"", Altitude = "-2°57'57\"" },
            new SkyPosition { Name = "Mars", RightAscension = "9h 19m 27s", Declination = "+17°13'04\"", Azimuth = "321°16'48\"", Altitude = "-17°59'56\"" },
            new SkyPosition { Name = "Jupiter", RightAscension = "9h 45m 48s", Declination = "+14°11'27\"", Azimuth = "313°54'56\"", Altitude = "-17°32'38\"" },
            new SkyPosition { Name = "Saturn", RightAscension = "0h 39m 06s", Declination = "+1°17'05\"", Azimuth = "90°13'33\"", Altitude = "+2°00'46\"" },
            new SkyPosition { Name = "Uranus", RightAscension = "4h 11m 33s", Declination = "+20°56'42\"", Azimuth = "36°54'03\"", Altitude = "-14°42'17\"" },
            new SkyPosition { Name = "Neptune", RightAscension = "0h 10m 07s", Declination = "-0°26'44\"", Azimuth = "96°38'56\"", Altitude = "+5°48'35\"" },
            new SkyPosition { Name = "Pluto", RightAscension = "20h 25m 53s", Declination = "-23°36'56\"", Azimuth = "155°59'51\"", Altitude = "+16°56'23\"" }
        };
    }
}
