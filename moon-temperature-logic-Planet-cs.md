# Moon Temperature Calculation in `Planet.cs`

## Objective

Add a method to `Planet.cs` that estimates a moon's **radiative equilibrium temperature** using the parent planet's average orbital distance from the Sun and the moon's assumed Bond albedo and thermal emissivity.

> **Important:** This estimates an idealized equilibrium temperature. It does not calculate the moon's measured average surface temperature. The Solar System OpenData API fields shown for Neptune and Triton do not include Triton's albedo or emissivity, so those inputs must come from a reliable scientific source or be explicitly treated as assumptions.

## Data from the API

For Neptune:

- `semimajorAxis = 4498396441` kilometres (Neptune's approximate average distance from the Sun).
- `avgTemp = 55` — do **not** use this value as the moon's temperature or as an input to this calculation.

For Triton:

- `semimajorAxis = 354760` kilometres (Triton's distance from Neptune).
- `avgTemp = 0` — this may indicate missing/unavailable data in this API; it should not be interpreted as 0°C.

Because Triton's orbit around Neptune is tiny compared with Neptune's distance from the Sun, this simplified calculation treats Triton as being at Neptune's distance from the Sun. The moon's `semimajorAxis` around its planet is not needed for this approximation.

## Formula

First estimate sunlight at Neptune's distance:

```text
solarRadiation = solarConstant / distanceAu²
distanceAu = planetDistanceFromSunKm / kmPerAu
```

Then calculate equilibrium temperature:

```text
temperatureKelvin =
    ((solarRadiation * (1 - albedo)) /
     (4 * emissivity * stefanBoltzmann)) ^ 0.25

temperatureCelsius = temperatureKelvin - 273.15
```

Constants:

- Solar constant at 1 AU: `1361 W/m²`
- 1 astronomical unit (AU): `149597870.7 km`
- Stefan–Boltzmann constant: `5.670374419 × 10⁻⁸ W/m²/K⁴`

`albedo` and `emissivity` must be provided for the moon. Albedo must be between 0 and 1; emissivity must be greater than 0 and at most 1.

## Suggested implementation

Add this method to your `Planet.cs` class, or preferably to a dedicated temperature-calculation service if `Planet.cs` is an API model/DTO.

```csharp
public static double CalculateMoonEquilibriumTemperatureCelsius(
    double planetDistanceFromSunKm,
    double moonBondAlbedo,
    double moonThermalEmissivity = 1.0)
{
    const double solarConstantWPerM2 = 1361.0;
    const double kilometersPerAu = 149597870.7;
    const double stefanBoltzmannWPerM2K4 = 5.670374419e-8;

    if (!double.IsFinite(planetDistanceFromSunKm) ||
        planetDistanceFromSunKm <= 0)
    {
        throw new ArgumentOutOfRangeException(
            nameof(planetDistanceFromSunKm),
            "Distance from the Sun must be a finite positive value.");
    }

    if (!double.IsFinite(moonBondAlbedo) ||
        moonBondAlbedo < 0 || moonBondAlbedo > 1)
    {
        throw new ArgumentOutOfRangeException(
            nameof(moonBondAlbedo),
            "Bond albedo must be between 0 and 1.");
    }

    if (!double.IsFinite(moonThermalEmissivity) ||
        moonThermalEmissivity <= 0 || moonThermalEmissivity > 1)
    {
        throw new ArgumentOutOfRangeException(
            nameof(moonThermalEmissivity),
            "Thermal emissivity must be greater than 0 and at most 1.");
    }

    double distanceAu = planetDistanceFromSunKm / kilometersPerAu;
    double solarRadiationWPerM2 =
        solarConstantWPerM2 / Math.Pow(distanceAu, 2);

    double temperatureKelvin = Math.Pow(
        (solarRadiationWPerM2 * (1.0 - moonBondAlbedo)) /
        (4.0 * moonThermalEmissivity * stefanBoltzmannWPerM2K4),
        0.25);

    return temperatureKelvin - 273.15;
}
```

This code uses `double.IsFinite`, which is available in modern .NET versions. If your target framework does not support it, validate for `double.IsNaN` and `double.IsInfinity` instead.

## Example call for Triton

The values below are **illustrative assumptions only**, not values returned by the API or guaranteed measured values for Triton.

```csharp
double neptuneDistanceFromSunKm = 4498396441.0;
double assumedTritonBondAlbedo = 0.80;
double assumedTritonEmissivity = 0.90;

double estimatedCelsius = CalculateMoonEquilibriumTemperatureCelsius(
    neptuneDistanceFromSunKm,
    assumedTritonBondAlbedo,
    assumedTritonEmissivity);

Console.WriteLine(
    $"Triton's estimated equilibrium temperature: {estimatedCelsius:F2} °C");
```

With these assumptions, the result is approximately **−228°C**. The result will change if the albedo or emissivity changes.

## Recommended integration in `Planet.cs`

If `Planet.cs` is a model representing API JSON, avoid mixing calculation logic into the model when possible. A cleaner design is:

1. Keep `Planet.cs` for API data properties such as `SemimajorAxis`, `AvgTemp`, and `Moons`.
2. Keep moon properties in a corresponding `Moon`/celestial-body model.
3. Put this calculation in a service such as `MoonTemperatureCalculator.cs`.
4. Pass the parent planet's `SemimajorAxis` and scientifically sourced moon albedo/emissivity to the service.
5. Label the result as an **estimated equilibrium temperature**, not an observed average temperature.

If you do place the method inside `Planet.cs`, make it `static` as shown and call it with Neptune's `SemimajorAxis`.

## Limitations

- This is a simplified equilibrium model, not a detailed surface-temperature model.
- It assumes the moon receives approximately the same sunlight as its parent planet.
- It does not account for atmospheric greenhouse effects, seasonal effects, internal heating, local day/night changes, or complex surface thermal behaviour.
- Do not treat `avgTemp: 0` as proof of 0°C. Check the API's field documentation or use a nullable value / explicit missing-data handling.
- Do not assume Neptune's `avgTemp: 55` is in Celsius unless the API documentation confirms the unit.

## Suggested display

Use labels that make the data quality clear:

- `Estimated equilibrium temperature: -228°C` — only when the inputs and assumptions are known.
- `Temperature unavailable` — when required inputs are missing.
- `Observed average surface temperature` — only when using a trusted observational/scientific source.
