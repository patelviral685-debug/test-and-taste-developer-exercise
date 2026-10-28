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

The API does not require authentication for this exercise.

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

The assessment asks for a motivation of changes made to the domain model and API interface. Document the actual changes after implementation, including:

- Which properties or methods were added or changed.
- How the moon-to-planet relationship is represented.
- How average temperature is calculated.
- How missing or invalid temperature data is handled.
- Why the API client or interface needed to change.

Do not claim changes that were not made.

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

- [ ] Existing functionality still works.
- [ ] Only planets with at least one moon are included in the new output.
- [ ] Average moon temperature is calculated from API data.
- [ ] Missing temperature data is handled deliberately.
- [ ] Console output is clear.
- [ ] Results are written to a file.
- [ ] API and file errors are handled.
- [ ] Code is separated into sensible classes.
- [ ] Comments explain important decisions.
- [ ] Regular commits exist on `TenForce-yourinitials`.
- [ ] Domain-model and API-interface changes are explained.
- [ ] The working branch is merged into `main`.
- [ ] A public repository link is ready to share.

## Assessment Note

The instructions state that finishing every part or producing highly optimized code is not the main goal. The emphasis is on demonstrating problem-solving, clear code organization, and explaining design decisions. Confirm with the recruiter whether internet or AI assistance is permitted before using such tools during the assessment.
