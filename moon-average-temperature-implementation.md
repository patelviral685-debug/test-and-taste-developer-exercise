# Implementing Moon Average Temperature Calculation in C#

## Goal

Use two separate API responses from the Solar System OpenData API:

- Planet response: Earth (`terre`)
- Moon response: Moon (`lune`)

Estimate the Moon's **solar equilibrium temperature** in Celsius.

> Important: This calculation estimates an idealized equilibrium temperature. It does **not** calculate the Moon's measured or true average surface temperature. The Moon has large day/night temperature variations, and the provided API data does not include all the physical inputs needed for a realistic thermal model.

## 1. API fields

### Earth (planet) data

Use:

- `id`: `"terre"` — identifies Earth.
- `semimajorAxis`: `149598023` km — Earth's approximate mean distance from the Sun.

Do not use Earth's `avgTemp` as the Moon's temperature.

### Moon data

Use:

- `id`: `"lune"` — identifies the Moon.
- `aroundPlanet.planet`: `"terre"` — identifies Earth as the parent planet.
- `avgTemp`: `0` — do not treat this as a measured temperature; it may indicate that the value is unavailable.
- `semimajorAxis`: `384400` km — Moon's approximate orbital distance from Earth. **This is not its distance from the Sun**, so it is not used in the simplified solar-equilibrium formula below.

### Additional calculation inputs

These are not present in the supplied API responses, so the implementation uses illustrative defaults:

- Moon albedo: `0.12`
- Emissivity: `0.90`
- Solar constant at 1 AU: `1361 W/m²`
- Stefan–Boltzmann constant: `5.670374419 × 10⁻⁸ W/m²/K⁴`
- 1 astronomical unit (AU): `149597870.7 km`

For scientific or production use, verify albedo and emissivity from a reliable scientific source and document the assumptions.

## 2. Formula

Convert the planet's distance from the Sun into astronomical units:

```text
distanceAU = planetSemimajorAxisKm / 149597870.7
```

Calculate idealized equilibrium temperature:

```text
temperatureKelvin =
    ((solarConstant * (1 - albedo)) /
    (4 * emissivity * stefanBoltzmannConstant * distanceAU²)) ^ 0.25
```

Convert Kelvin to Celsius:

```text
temperatureCelsius = temperatureKelvin - 273.15
```

The result with the example assumptions is approximately **0°C**. This is a simplified theoretical estimate, not the Moon's actual average surface temperature.

## 3. Suggested implementation

Keep the calculation in a service/helper class rather than in the API DTO/model. This keeps API data models focused on representing API responses.

Create `MoonTemperatureCalculator.cs`:

```csharp
using System;

public static class MoonTemperatureCalculator
{
    private const double SolarConstant = 1361.0;
    private const double AstronomicalUnitKm = 149597870.7;
    private const double StefanBoltzmannConstant = 5.670374419e-8;

    public static double CalculateEquilibriumTemperatureCelsius(
        double planetDistanceFromSunKm,
        double moonAlbedo = 0.12,
        double emissivity = 0.90)
    {
        if (planetDistanceFromSunKm <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(planetDistanceFromSunKm),
                "Distance from the Sun must be greater than zero.");

        if (moonAlbedo < 0 || moonAlbedo > 1)
            throw new ArgumentOutOfRangeException(
                nameof(moonAlbedo),
                "Albedo must be between 0 and 1.");

        if (emissivity <= 0 || emissivity > 1)
            throw new ArgumentOutOfRangeException(
                nameof(emissivity),
                "Emissivity must be greater than 0 and no greater than 1.");

        double distanceAu = planetDistanceFromSunKm / AstronomicalUnitKm;

        double temperatureKelvin = Math.Pow(
            (SolarConstant * (1.0 - moonAlbedo)) /
            (4.0 * emissivity * StefanBoltzmannConstant *
             Math.Pow(distanceAu, 2)),
            0.25);

        return temperatureKelvin - 273.15;
    }
}
```

## 4. Example usage with the separate API responses

After deserializing the API responses into your own models, pass the planet's `semimajorAxis` to the calculator:

```csharp
double earthDistanceFromSunKm = earth.semimajorAxis;

double estimatedMoonTemperatureCelsius =
    MoonTemperatureCalculator.CalculateEquilibriumTemperatureCelsius(
        earthDistanceFromSunKm);

Console.WriteLine(
    $"Estimated Moon equilibrium temperature: " +
    $"{estimatedMoonTemperatureCelsius:F2} °C");
```

Here, `earth` represents the deserialized Earth response. Adapt the variable and property names to your actual C# model types.

## 5. Match the moon to its parent planet

Because the API responses are separate, you can use the Moon's `aroundPlanet.planet` value to find the matching planet:

```csharp
var parentPlanet = planets.FirstOrDefault(
    p => string.Equals(
        p.id,
        moon.aroundPlanet?.planet,
        StringComparison.OrdinalIgnoreCase));

if (parentPlanet == null)
{
    throw new InvalidOperationException(
        $"Parent planet '{moon.aroundPlanet?.planet}' was not found.");
}

double temperatureCelsius =
    MoonTemperatureCalculator.CalculateEquilibriumTemperatureCelsius(
        parentPlanet.semimajorAxis);
```

Add this namespace if needed:

```csharp
using System;
using System.Linq;
```

This example assumes your deserialized models expose `id`, `semimajorAxis`, and `aroundPlanet.planet` with compatible C# property names.

## 6. Which fields are used?

| Field/input | Used? | Reason |
|---|---|---|
| Planet `semimajorAxis` | Yes | Approximates the planet's distance from the Sun, used as the Moon's solar distance. |
| Moon `aroundPlanet.planet` | For matching only | Finds the parent planet record. |
| Moon `semimajorAxis` | No, not in this formula | It is the Moon's distance from its planet. |
| Planet `avgTemp` | No | Earth's temperature is not the Moon's temperature. |
| Moon `avgTemp` | No | `0` may represent missing/unavailable data. |
| `meanRadius`, `mass`, `density`, `gravity` | No | Not needed by this simplified formula. |
| Moon albedo | Yes, as an assumption | Fraction of sunlight reflected by the Moon. |
| Emissivity | Yes, as an assumption | Describes thermal radiation emitted by the surface. |

## 7. Limitations and naming

- Prefer naming the result `EstimatedEquilibriumTemperatureCelsius`, not `AverageTemperature`, because the calculation is an idealized equilibrium estimate.
- The planet's `semimajorAxis` is an approximation of the Moon's solar distance. A more precise calculation would use the Moon's actual position relative to the Sun at a specified time.
- This formula does not model the Moon's rotation, surface thermal inertia, latitude, shadowing, or day/night cycles.
- If the API provides a verified, measured average temperature for the Moon, use that source value instead of pretending this estimate is a measured average.
- The defaults are illustrative assumptions and should be reviewed before use in scientific or production applications.
