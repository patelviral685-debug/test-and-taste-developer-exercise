# Estimate the Moon's Current Temperature from Static Astronomical Data

## Goal

Implement C# logic that uses the supplied static Sun, Moon, and planet position data to estimate the Moon's surface temperature.

**Important limitation:** the provided right ascension, declination, azimuth, and altitude values describe objects' positions in the sky as seen by an observer on Earth. They do not directly give the Sun's angle relative to a particular point on the Moon. Therefore, these values alone cannot determine the Moon's current surface temperature. This guide implements a transparent **rough estimate** using the Sun's altitude as a proxy for solar elevation, and clearly labels the result as a proxy rather than a measured lunar temperature.

For a scientifically meaningful lunar surface estimate, calculate the Sun's incidence angle at a specified lunar latitude/longitude and use a thermal model that accounts for surface properties and heat storage.

## 1. Static input data

The supplied values are:

| Object | Right Ascension | Declination | Azimuth | Altitude |
|---|---|---|---|---|
| Sun (`Soleil`) | 14h 11m 51s | −13°16′16″ | 245°14′59″ | +4°54′32″ |
| Moon (`Lune`) | 4h 28m 18s | +25°48′34″ | 31°12′09″ | −11°56′05″ |
| Mercury | 15h 05m 53s | −19°55′38″ | 230°52′27″ | +7°38′34″ |
| Venus | 13h 35m 46s | −16°02′02″ | 249°55′07″ | −2°57′57″ |
| Mars | 9h 19m 27s | +17°13′04″ | 321°16′48″ | −17°59′56″ |
| Jupiter | 9h 45m 48s | +14°11′27″ | 313°54′56″ | −17°32′38″ |
| Saturn | 0h 39m 06s | +1°17′05″ | 90°13′33″ | +2°00′46″ |
| Uranus | 4h 11m 33s | +20°56′42″ | 36°54′03″ | −14°42′17″ |
| Neptune | 0h 10m 07s | −0°26′44″ | 96°38′56″ | +5°48′35″ |
| Pluto | 20h 25m 53s | −23°36′56″ | 155°59′51″ | +16°56′23″ |

The Sun altitude is about `+4.909°`. The Moon altitude is about `−11.935°`. This means the Sun is slightly above the observer's horizon while the Moon is below it. **The Moon being below the observer's horizon does not mean the Moon's surface is unlit.**

## 2. What is needed for a temperature estimate?

A simplified instantaneous radiative-equilibrium estimate for a lunar surface patch is:

```text
T_kelvin = ((1 - albedo) * solarFlux * max(0, cos(incidenceAngle))
            / (emissivity * StefanBoltzmannConstant)) ^ 0.25

T_celsius = T_kelvin - 273.15
```

Inputs:

- `solarFlux`: sunlight at the Moon in W/m²; roughly 1361 W/m² near 1 AU.
- `albedo`: fraction of sunlight reflected; use a documented assumption, e.g. `0.12`, for a rough example.
- `emissivity`: thermal emissivity; use a documented assumption, e.g. `0.95`, for a rough example.
- `incidenceAngle`: angle between incoming sunlight and the local surface normal at the lunar point being modeled.
- `StefanBoltzmannConstant`: `5.670374419e-8 W/m²/K⁴`.

**The supplied Sun altitude is measured from the Earth observer's horizon, not from the lunar surface.** It must not be presented as the Moon's true incidence angle. If it is used as a rough proxy for a demo, the output must be labeled `proxy`, not `current lunar surface temperature`.

The formula also ignores lunar regolith heat storage, rotation, local slope, latitude, shadows, and the difference between day-side and night-side conditions. It can overestimate the temperature of a real surface because it assumes the patch immediately reaches radiative equilibrium.

## 3. C# model for the static data

```csharp
public sealed class SkyPosition
{
    public string Name { get; set; } = string.Empty;
    public string RightAscension { get; set; } = string.Empty;
    public string Declination { get; set; } = string.Empty;
    public string Azimuth { get; set; } = string.Empty;
    public string Altitude { get; set; } = string.Empty;
}
```

Example data for the Sun and Moon:

```csharp
var sun = new SkyPosition
{
    Name = "Soleil",
    RightAscension = "14h 11min 51s",
    Declination = "-13°16'16\\"",
    Azimuth = "245°14'59\\"",
    Altitude = "4°54'32\\""
};

var moon = new SkyPosition
{
    Name = "Lune",
    RightAscension = "4h 28min 18s",
    Declination = "25°48'34\\"",
    Azimuth = "31°12'09\\"",
    Altitude = "-11°56'05\\""
};
```

## 4. Parse a signed angle in degrees, minutes, and seconds

The angle strings include a sign and degrees/minutes/seconds. Parse them once, rather than embedding the values directly in the calculation.

```csharp
using System;
using System.Globalization;
using System.Text.RegularExpressions;

public static class AngleParser
{
    private static readonly Regex DmsPattern = new(
        @"^\s*(?<sign>[+-])?\s*(?<deg>\d+)\s*°\s*(?<min>\d+)\s*['′]\s*(?<sec>\d+(?:\.\d+)?)\s*[""″]?\s*$",
        RegexOptions.Compiled);

    public static double ParseDegrees(string value)
    {
        var match = DmsPattern.Match(value);

        if (!match.Success)
            throw new FormatException($"Invalid angle: '{value}'");

        double degrees = double.Parse(
            match.Groups["deg"].Value, CultureInfo.InvariantCulture);
        double minutes = double.Parse(
            match.Groups["min"].Value, CultureInfo.InvariantCulture);
        double seconds = double.Parse(
            match.Groups["sec"].Value, CultureInfo.InvariantCulture);

        double result = degrees + minutes / 60.0 + seconds / 3600.0;

        if (match.Groups["sign"].Value == "-")
            result = -result;

        return result;
    }
}
```

If the input format changes, adjust the parser or store angles as numeric degrees in your application model.

## 5. A clearly labelled proxy calculation

This demo uses the Sun's **Earth-observer altitude** as a proxy for incidence angle. It is included only to demonstrate how the static input can flow through code; it is **not a physically reliable lunar temperature calculation**.

```csharp
using System;

public static class MoonTemperatureEstimator
{
    private const double SolarFluxAtOneAu = 1361.0; // W/m²
    private const double StefanBoltzmannConstant = 5.670374419e-8;
    private const double DefaultAlbedo = 0.12;
    private const double DefaultEmissivity = 0.95;

    public static double EstimateProxyCelsiusFromSunAltitude(
        double sunAltitudeDegrees,
        double albedo = DefaultAlbedo,
        double emissivity = DefaultEmissivity)
    {
        if (albedo < 0 || albedo > 1)
            throw new ArgumentOutOfRangeException(nameof(albedo));

        if (emissivity <= 0 || emissivity > 1)
            throw new ArgumentOutOfRangeException(nameof(emissivity));

        // DEMO ONLY: this is an observer-based altitude, not the
        // incidence angle at a specific location on the Moon.
        double incidenceProxyRadians = sunAltitudeDegrees * Math.PI / 180.0;
        double illuminationProxy = Math.Max(
            0.0, Math.Cos(incidenceProxyRadians));

        if (illuminationProxy == 0)
        {
            // This only says the proxy sees no sunlight when the Sun
            // is below the observer's horizon; it does NOT establish
            // that the Moon's surface is in darkness.
            throw new InvalidOperationException(
                "The Sun is below the observer's horizon. " +
                "This proxy cannot estimate lunar surface temperature.");
        }

        double absorbedFlux =
            SolarFluxAtOneAu * (1.0 - albedo) * illuminationProxy;

        double temperatureKelvin = Math.Pow(
            absorbedFlux / (emissivity * StefanBoltzmannConstant),
            0.25);

        return temperatureKelvin - 273.15;
    }
}
```

Example call:

```csharp
double sunAltitudeDegrees = AngleParser.ParseDegrees(sun.Altitude);

double proxyCelsius =
    MoonTemperatureEstimator.EstimateProxyCelsiusFromSunAltitude(
        sunAltitudeDegrees);

Console.WriteLine(
    $"Illustrative temperature proxy: {proxyCelsius:F1} °C " +
    "(not the Moon's actual current surface temperature)");
```

Because the Sun is only about 4.9° above the Earth observer's horizon, `cos(sunAltitude)` is close to 1. This proxy would therefore produce a high sunlit equilibrium value. **Do not interpret that output as the Moon's actual temperature.** The angle is geometrically unrelated to the incidence angle on a particular lunar surface point.

## 6. Recommended production logic

For a defensible current estimate:

1. Record the exact UTC date/time for the sky-position data.
2. Obtain a lunar ephemeris that provides the Sun's direction relative to the Moon at that same time.
3. Choose a lunar surface location (latitude and longitude), or explicitly calculate the subsolar point.
4. Calculate the angle between the solar ray and that location's surface normal.
5. Obtain documented lunar albedo, emissivity, and preferably thermal inertia/thermal-model data.
6. Use a lunar surface thermal model to account for heat storage and local time, not just instantaneous radiative equilibrium.
7. Return a value with metadata such as `estimateType`, `assumptions`, `timestampUtc`, and `locationOnMoon`.

Suggested result shape:

```json
{
  "body": "Moon",
  "temperatureCelsius": null,
  "estimateType": "requires-lunar-incidence-angle-and-thermal-model",
  "timestampUtc": null,
  "lunarLatitudeDegrees": null,
  "lunarLongitudeDegrees": null,
  "assumptions": [
    "Sky altitudes are observer-based and cannot directly determine lunar surface temperature."
  ]
}
```

## 7. Key takeaway

Your static sky-position data is useful for describing where the objects appear from Earth. The Sun's altitude and azimuth alone cannot determine the Moon's current surface temperature. Use this data for a demo or visualization, but for a scientifically meaningful estimate, calculate the solar incidence angle at a specified point on the Moon and use a lunar thermal model.
