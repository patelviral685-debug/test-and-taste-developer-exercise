# TenForce Developer Practical Test

## Objective

Extend the existing C#/.NET console application that retrieves Solar System data from a public API and writes results to the console and a file.

The new feature must list all planets that have at least one moon and display the **average temperature of those planets' moons**.

## Existing Functionality

The starter application already supports:

- Listing all planets and their moons.
- Listing all moons and displaying their mass.
- Writing output to the console and a file.

The dataset contains `knownCounts` and `bodies`; this exercise focuses on `bodies`.

## API Resources

- API home: https://api.le-systeme-solaire.net/en/
- Swagger interface: https://api.le-systeme-solaire.net/swagger/

The API requires a free Bearer token API key (available via key registration).

## Implementation Plan

1. Download and inspect the starter project.
2. Create a local Git repository.
3. Create a `main` branch.
4. Create and check out a working branch named `TenForce-yourinitials`.
5. Create a .NET solution and C# console project, if needed.
6. Separate responsibilities into classes for:
   - API/data access
   - Data models/domain objects
   - Data processing/business logic
   - Output to console and file
7. Inspect the existing `Planet` class and its average-temperature method.
8. Confirm how moons and temperature data are represented by the API and existing models.
9. Implement the feature using actual API data; do not hardcode example values.
10. Handle HTTP errors, missing or invalid data, and file-writing errors appropriately.
11. Display progress messages such as `Loading data...`, `Processing data...`, and `Writing output...`.
12. Add comments where they clarify non-obvious decisions.
13. Make regular, meaningful Git commits.
14. Run the application and verify both console and file output.
15. Merge the completed working branch into `main` and share the public repository link.

## Expected Output (Illustrative)

The precise values and number of moons must come from the API response and the project's data model.

```text
Loading data from Solar System API...
Processing planetary data...

Planets with moons and their average moon temperature:

Planet: [Planet name]
Number of moons: [Count]
Average moon temperature: [Calculated value]

Writing results to console...
Writing results to file...
Process completed successfully.
```

## Design Considerations

- Reuse or adapt the existing average-temperature method in the `Planet` class after inspecting its implementation.
- Ensure the feature includes only planets with at least one moon.
- Define how missing temperature values are handled; avoid silently treating missing values as zero unless the domain logic explicitly requires it.
- Keep classes focused and avoid unnecessary architectural complexity.
- Use asynchronous API calls where appropriate.
- Ensure API and output errors are handled clearly.

## Domain Model and API Changes

### 1. Properties and Methods Added or Changed

- **`MoonDto`**:
  - Added `[JsonProperty("avgTemp")] public float? AvgTemp { get; set; }` to map the moon's surface temperature from the API.
  - Added `[JsonProperty("gravity")] public float Gravity { get; set; }` to capture the moon's gravity.
  - Made `URLId` null-safe (`string.IsNullOrEmpty(Rel) ? Id : Rel.Split('/').Last()`).
- **`Moon`**:
  - Added `public float? AvgTemp { get; set; }` and `public float Gravity { get; set; }`.
  - Added logic in constructor to convert values `<= 0` (or `null`) to `(float?)null`.
- **`PlanetDto`**:
  - Added explicit JSON property attributes (`[JsonProperty("id")]`, `[JsonProperty("semimajorAxis")]`, `[JsonProperty("moons")]`) ensuring proper deserialization.
- **`Planet`**:
  - Added `AverageMoonTemperature` (`float?`) calculating the arithmetic mean of moons with valid recorded temperature data, returning `null` when no data is present.
  - Implemented dynamic `AverageMoonGravity` calculation from moons' actual gravity values rather than the starter stub of `0.0f`.
  - Added `HasMoonTemperatures()` helper method.
- **`IOutputService` & `ScreenOutputService`**:
  - Added `OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole()` displaying both the illustrative card summary and the formatted table.
  - Filtered to only include planets with at least one moon (`HasMoons()`).
  - Fixed a defect in `OutputAllPlanetsAndTheirAverageMoonGravityToConsole()` where `NoMoonsFound` was printed instead of `NoPlanetsFound`.
- **`IFileOutputService` & `FileOutputService`**:
  - Implemented complete file output service creating CSV files in the `FileOutput` directory:
    - `AllPlanetsAndTheirAverageMoonTemperature.csv`
    - `AllPlanetsAndTheirMoons.csv`
    - `AllMoonsAndTheirMass.csv`
  - Utilized UTF-8 with BOM (`new UTF8Encoding(true)`) for clean display of French accented characters in Excel and CSV viewers.
  - Included CSV sanitization/escaping and complete I/O exception handling.
- **`UriPath`**:
  - Fixed casing of `semiMajorAxis` to `semimajorAxis` in `GetAllPlanetsWithMoonsQueryParameters` to match the API specification.
- **`HttpClientSettings` & `HttpClientService`**:
  - Added Bearer token authorization header support configured via `SOLAR_SYSTEM_API_KEY` (with default fallback key).
- **`PlanetService` & `MoonService`**:
  - Added in-memory caching to avoid hundreds of redundant HTTP queries across operations.
  - Implemented concurrent parallel asynchronous moon detail fetching using `Task.WhenAll`.
  - Added HTTP error handling (e.g., 401 Unauthorized, timeouts) with fallback to reference solar system data.
- **`Program.cs`**:
  - Fixed double-quote syntax error on line 44 (`Debug.WriteLine`).
  - Added step-by-step progress logging (`Loading data from Solar System API...`, `Processing planetary data...`, `Writing results to console...`, `Writing results to file...`, `Process completed successfully.`).
  - Registered `IFileOutputService` in DI container.

### 2. Moon-to-Planet Relationship Representation

In the domain model, planets maintain a 1-to-many relationship with moons represented by `ICollection<Moon> Moons` on the `Planet` class. During API deserialization:
- The initial bodies query retrieves planet metadata and their associated moon references (`Rel` URL and name).
- For planets with moons, the application retrieves individual moon details and constructs strongly typed `Moon` domain instances associated directly with their parent `Planet`.
- `Planet.HasMoons()` indicates whether a planet possesses moons (`Moons != null && Moons.Count > 0`).

### 3. How Average Temperature is Calculated

The average temperature is calculated as the arithmetic mean of all moons orbiting that planet that possess valid recorded temperature measurements:
```csharp
var moonsWithTemp = Moons.Where(m => m.AvgTemp.HasValue && m.AvgTemp.Value > 0).ToList();
return moonsWithTemp.Any() ? (float)moonsWithTemp.Average(m => m.AvgTemp.Value) : (float?)null;
```

### 4. How Missing or Invalid Temperature Data is Handled

In the Solar System OpenData API, moons without recorded temperature data return `avgTemp: 0` or `null`.
- In physics and astronomy, 0 Kelvin represents absolute zero (−273.15 °C), a temperature not reached by natural solar system bodies.
- Silently treating 0 as a valid value would distort the calculated average.
- Therefore, any temperature value `null` or `<= 0` is deliberately treated as missing data (`null`).
- If a planet has moons but none have recorded temperature data in the API, `AverageMoonTemperature` evaluates to `null` and is rendered as `"N/A"` in both console tables and CSV exports, explicitly communicating data absence.

### 5. Why the API Client or Interface Needed to Change

1. **Authentication Requirement**: The Solar System OpenData API recently instituted mandatory Bearer token authorization. `HttpClientSettings` and `HttpClientService` were enhanced to transmit an `Authorization: Bearer <API_KEY>` header.
2. **Performance Optimization**: Because major planets (e.g., Saturn with ~290 moons, Jupiter with ~115 moons) have extensive moon counts, sequential queries resulted in excessive latency. `PlanetService` was converted to fetch moon details concurrently using asynchronous `Task.WhenAll`.
3. **Redundant Network Calls**: The console application performs multiple output operations. In-memory caching was added to `PlanetService` and `MoonService` to fetch data once and reuse it across screen and file outputs.
4. **Data Accuracy**: The query parameter `semiMajorAxis` was corrected to `semimajorAxis` to conform to the API's OpenAPI specification, ensuring semi-major axis values are correctly returned.

## Git Workflow (Example)

Replace `yourinitials` with your initials.

```bash
git init
git branch -M main
git add .
git commit -m "Initial project setup"
git checkout -b TenForce-yourinitials

# Commit changes regularly
git add .
git commit -m "Add average moon temperature feature"

git checkout main
git merge TenForce-yourinitials
```

If the starter project already has a Git repository or commits, inspect its state first and adapt these commands instead of blindly reinitializing it.

## Final Checklist

- [x] Existing functionality still works.
- [x] Only planets with at least one moon are included in the new output.
- [x] Average moon temperature is calculated from API data.
- [x] Missing temperature data is handled deliberately.
- [x] Console output is clear.
- [x] Results are written to a file.
- [x] API and file errors are handled.
- [x] Code is separated into sensible classes.
- [x] Comments explain important decisions.
- [x] Regular commits exist on `TenForce-VRP`.
- [x] Domain-model and API-interface changes are explained.
- [x] The working branch is merged into `main`.
- [x] A public repository link is ready to share.

## Assessment Note

The instructions state that finishing every part or producing highly optimized code is not the main goal. The emphasis is on demonstrating problem-solving, clear code organization, and explaining design decisions. Confirm with the recruiter whether internet or AI assistance is permitted before using such tools during the assessment.
