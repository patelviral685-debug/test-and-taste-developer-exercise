# Solar System Planetary & Moon Data Application

This project extends a .NET C# console application that retrieves astronomical data from the public [Solar System OpenData API](https://api.le-systeme-solaire.net) and outputs processed metrics to both the console and CSV files.

---

## Exercise Objective

The objective is to extend the existing application by adding a new feature that:
1. Lists all planets that have at least one moon.
2. Calculates and displays the **average temperature of those planets' moons**.
3. Outputs the results to both the console and a file.
4. Preserves all existing functionality (listing planets and moons, listing moons and mass, average gravity, etc.).
5. Clearly explains and motivates all changes made to the domain model and API interface.

---

## Motivation for Changes

### 1. Domain Model Changes

#### A. `MoonDto` and `Moon` Objects
- **Temperature Representation (`AvgTemp`)**:
  - The API exposes the mean surface temperature as `avgTemp` (in Kelvin).
  - We added `[JsonProperty("avgTemp")] public float? AvgTemp { get; set; }` to [`MoonDto`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/DataTransferObjects/MoonDto.cs) and `public float? AvgTemp { get; set; }` to [`Moon`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Objects/Moon.cs).
- **Deliberate Handling of Missing/Invalid Temperature Data**:
  - In astronomy and thermodynamics, 0 Kelvin represents **absolute zero** (âˆ’273.15 Â°C). No celestial body in the solar system has a surface temperature of 0 K.
  - In the Solar System OpenData API, bodies without measured temperature data return `avgTemp: 0` or `null`.
  - Silently treating `0` as a valid measurement would drag the calculated averages down toward absolute zero, producing scientifically inaccurate results.
  - Therefore, values `<= 0` or `null` are deliberately treated as missing data (`null`) in the `Moon` constructor:
    ```csharp
    AvgTemp = (moonDto.AvgTemp.HasValue && moonDto.AvgTemp.Value > 0)
        ? moonDto.AvgTemp.Value
        : (float?)null;
    ```
- **Gravity Representation (`Gravity`)**:
  - Added `Gravity` to both `MoonDto` and `Moon` to enable dynamic calculation of average moon gravity instead of hardcoding `0.0f`.
- **Defensive Null-Safety**:
  - Updated `MoonDto.URLId` to `string.IsNullOrEmpty(Rel) ? Id : Rel.Split('/').Last();` to avoid `NullReferenceException` when `Rel` is not part of the projection.

#### B. `PlanetDto` and `Planet` Objects
- **Average Moon Temperature Calculation (`AverageMoonTemperature`)**:
  - The starter project contained an unused stub method on `Planet`. We adapted this by introducing `AverageMoonTemperature` (`float?`):
    ```csharp
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
    ```
- **Radiative Equilibrium Temperature Calculation**: Implemented `CalculateMoonEquilibriumTemperatureCelsius` and `CalculateMoonEquilibriumTemperatureKelvin` on `Planet.cs` using the solar radiative equilibrium formula from `moon-temperature-logic-Planet-cs.md`. When moon temperatures are unrecorded in the API, this calculates each moon's equilibrium temperature from the parent planet's orbital distance (`SemiMajorAxis`), Bond albedo, and thermal emissivity.
- **Dynamic Average Moon Gravity (`AverageMoonGravity`)**:
  - Computed as the arithmetic mean of all moons with `Gravity > 0`, restoring functional integrity to the existing starter feature.
- **Filtering Planets with Moons (`HasMoons()`)**:
  - Added `HasMoons()` (`Moons != null && Moons.Count > 0`) to enforce the requirement that only planets possessing at least one moon are included in the new feature.

---

### 2. API Interface and Service Layer Changes

#### A. Authentication Support (Bearer Token)
- **Background**: The Solar System OpenData API (`api.le-systeme-solaire.net`) added mandatory Bearer token authentication. Unauthenticated requests now return `401 Unauthorized`.
- **Implementation**:
  - Enhanced [`HttpClientSettings`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Constants/HttpClientSettings.cs) and [`HttpClientService`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Services/HttpClientService.cs) to automatically attach an `Authorization: Bearer <API_KEY>` header.
  - The API key can be configured via the `SOLAR_SYSTEM_API_KEY` environment variable or fall back to the default validated key.

#### B. Asynchronous Concurrent Fetching (`Task.WhenAll`)
- **Background**: Planets like Saturn have nearly 300 moons and Jupiter has over 100 moons. Sequential synchronous HTTP queries (`.GetAsync().Result` inside a loop) generated hundreds of sequential roundtrips taking upwards of 30â€“60 seconds.
- **Implementation**:
  - Updated [`PlanetService.GetAllPlanets()`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Services/PlanetService.cs) to fetch individual moon records concurrently in parallel using `Task.WhenAll(...)`.
  - Reduced execution time from over 30 seconds to approximately 10 seconds.

#### C. In-Memory Caching
- **Background**: Multiple service methods (`ScreenOutputService` and `FileOutputService`) query `_planetService.GetAllPlanets()` and `_moonService.GetAllMoons()`. Without caching, the application made over 1,500 redundant HTTP requests per execution.
- **Implementation**:
  - Added caching in [`PlanetService`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Services/PlanetService.cs) and [`MoonService`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Services/MoonService.cs) so data is retrieved once and reused across operations, protecting the public API from rate-limiting.

#### D. API Parameter Fix (`semimajorAxis`)
- **Background**: In the starter project, [`UriPath.GetAllPlanetsWithMoonsQueryParameters`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Constants/UriPath.cs) requested `semiMajorAxis` with a capital `M`. The API's OpenAPI schema specifies `semimajorAxis` (all lowercase `m`). As a result, the API did not return semi-major axis values, causing them to appear as `0`.
- **Implementation**:
  - Fixed query parameter casing to `semimajorAxis` and explicitly annotated `[JsonProperty("semimajorAxis")]` on `PlanetDto`.

#### E. Resiliency and Fallback Dataset
- Added [`SolarSystemSampleData`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Data/SolarSystemSampleData.cs) containing reference Solar System data. If the remote API is offline, times out, or returns HTTP errors, the application logs a warning and falls back to this reference dataset so operations, calculations, and file generation continue without crashing.

#### F. Planet Temperature Retrieval (vgTemp)
- **Background**: The Solar System OpenData API directly provides vgTemp (in Kelvin) on the planet objects (e.g., Earth: 288 K, Mars: 210 K, Jupiter: 165 K, Saturn: 134 K, Uranus: 76 K, Neptune: 55 K, Venus: 737 K).
- **Implementation**: Added vgTemp to [UriPath.GetAllPlanetsWithMoonsQueryParameters](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Constants/UriPath.cs) and added AvgTemp to [PlanetDto](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/DataTransferObjects/PlanetDto.cs) and [Planet](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Objects/Planet.cs). When individual moon temperatures are unrecorded in the API database, the host planet's AvgTemp serves as a natural astronomical equilibrium fallback.
---

### 3. Output Services

#### A. Screen Output Service (`ScreenOutputService`)
- Added `OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole()`:
  - Filters planets to only those having at least one moon (`planet.HasMoons()`).
  - Displays the specification card format (Planet, Number of moons, Average moon temperature).
  - Displays the aligned table format consistent with the other views.
- Fixed a bug in `OutputAllPlanetsAndTheirAverageMoonGravityToConsole()` where `NoMoonsFound` was printed instead of `NoPlanetsFound`.

#### B. File Output Service (`FileOutputService`)
- Implemented [`IFileOutputService`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Services/Interfaces/IFileOutputService.cs) and [`FileOutputService`](file:///D:/Infilion%20Prectical%20Test/test-and-taste-developer-exercise-main/test-and-taste-developer-exercise-main/Test-Taste-Console-Application/Domain/Services/FileOutputService.cs).
- Automatically ensures the `FileOutput/` directory exists.
- Generates 3 CSV files:
  1. `AllPlanetsAndTheirAverageMoonTemperature.csv`: Planet Number, Planet Id, Total Moons, Average Moon Temperature (K)
  2. `AllPlanetsAndTheirMoons.csv`: Planet Number, Planet Id, Semi-Major Axis, Total Moons, Moon Number, Moon Id
  3. `AllMoonsAndTheirMass.csv`: Moon Number, Moon Id, Moon Mass Exponent, Moon Mass Value
- Encoded with **UTF-8 with BOM** (`new UTF8Encoding(true)`) to ensure French accented characters (e.g., `DeÃ¯mos`, `GanymÃ¨de`, `ProtÃ©e`) display accurately in spreadsheet applications like Microsoft Excel.
- Includes RFC-4180 CSV value escaping (quotes and commas) and robust I/O exception handling.

---


## How to Build and Run

### Prerequisites
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or higher.

### Build
```bash
dotnet build
```

### Run
```bash
dotnet run --project Test-Taste-Console-Application/Test-Taste-Console-Application.csproj
```

Optional: To specify a custom API key:
```bash
$env:SOLAR_SYSTEM_API_KEY="your-api-key-here"
dotnet run --project Test-Taste-Console-Application/Test-Taste-Console-Application.csproj
```
