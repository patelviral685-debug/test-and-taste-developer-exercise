using System;
using System.Globalization;
using System.Linq;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class ScreenOutputService : IOutputService
    {
        private readonly IPlanetService _planetService;
        private readonly IMoonService _moonService;

        public ScreenOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        public void OutputAllPlanetsAndTheirMoonsToConsole()
        {
            // The service gets all the planets from the API.
            var planets = _planetService.GetAllPlanets().ToArray();

            // If the planets aren't found, then the function stops and tells that to the user via the console.
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            // The column sizes and labels for the planets are configured here.
            var columnSizesForPlanets = new[] { 20, 20, 30, 20 };
            var columnLabelsForPlanets = new[]
            {
                OutputString.PlanetNumber, OutputString.PlanetId, OutputString.PlanetSemiMajorAxis,
                OutputString.TotalMoons
            };

            // The column sizes and labels for the moons are configured here.
            var columnSizesForMoons = new[] { 20, 70 + 2 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId
            };

            for (int i = 0, j = 1; i < planets.Length; i++, j++)
            {
                ConsoleWriter.CreateLine(columnSizesForPlanets);
                ConsoleWriter.CreateText(columnLabelsForPlanets, columnSizesForPlanets);
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Id),
                        planets[i].SemiMajorAxis.ToString(CultureInfo.InvariantCulture),
                        planets[i].Moons.Count.ToString()
                    },
                    columnSizesForPlanets);

                ConsoleWriter.CreateLine(columnSizesForPlanets);
                ConsoleWriter.CreateText(columnLabelsForMoons, columnSizesForMoons);

                for (int k = 0, l = 1; k < planets[i].Moons.Count; k++, l++)
                {
                    ConsoleWriter.CreateText(
                        new[]
                        {
                            l.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Moons.ElementAt(k).Id)
                        },
                        columnSizesForMoons);
                }

                ConsoleWriter.CreateLine(columnSizesForMoons);
                ConsoleWriter.CreateEmptyLines(2);
            }
        }

        public void OutputAllMoonsAndTheirMassToConsole()
        {
            var moons = _moonService.GetAllMoons().ToArray();

            if (!moons.Any())
            {
                Console.WriteLine(OutputString.NoMoonsFound);
                return;
            }

            var columnSizesForMoons = new[] { 20, 20, 30, 20 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId, OutputString.MoonMassExponent, OutputString.MoonMassValue
            };

            ConsoleWriter.CreateHeader(columnLabelsForMoons, columnSizesForMoons);

            for (int i = 0, j = 1; i < moons.Length; i++, j++)
            {
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(moons[i].Id),
                        moons[i].MassExponent.ToString(CultureInfo.InvariantCulture),
                        moons[i].MassValue.ToString(CultureInfo.InvariantCulture)
                    },
                    columnSizesForMoons);
            }

            ConsoleWriter.CreateLine(columnSizesForMoons);
            ConsoleWriter.CreateEmptyLines(2);
        }

        public void OutputAllPlanetsAndTheirAverageMoonGravityToConsole()
        {
            var planets = _planetService.GetAllPlanets().ToArray();
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            var columnSizes = new[] { 20, 30 };
            var columnLabels = new[]
            {
                OutputString.PlanetId, OutputString.PlanetMoonAverageGravity
            };

            ConsoleWriter.CreateHeader(columnLabels, columnSizes);

            foreach (Planet planet in planets)
            {
                if (planet.HasMoons())
                {
                    ConsoleWriter.CreateText(
                        new[] { $"{planet.Id}", $"{planet.AverageMoonGravity.ToString("F4", CultureInfo.InvariantCulture)}" },
                        columnSizes);
                }
                else
                {
                    ConsoleWriter.CreateText(new[] { $"{planet.Id}", "-" }, columnSizes);
                }
            }

            ConsoleWriter.CreateLine(columnSizes);
            ConsoleWriter.CreateEmptyLines(2);
        }

        /// <inheritdoc />
        public void OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole()
        {
            // Only include planets that have at least one moon
            var planets = _planetService.GetAllPlanets()
                .Where(p => p.HasMoons())
                .ToArray();

            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            Console.WriteLine(OutputString.PlanetsWithMoonsAndAverageTemperatureHeader);
            Console.WriteLine();

            // Display formatted cards matching TenForce specification
            foreach (var planet in planets)
            {
                var planetName = CultureInfoUtility.TextInfo.ToTitleCase(planet.Id);
                var moonCount = planet.Moons.Count;
                var avgTempStr = planet.AverageMoonTemperature.HasValue
                    ? $"{planet.AverageMoonTemperature.Value.ToString("F2", CultureInfo.InvariantCulture)} K"
                    : OutputString.NotAvailable;

                Console.WriteLine($"{OutputString.PlanetPrefix}{planetName}");
                Console.WriteLine($"{OutputString.NumberOfMoonsPrefix}{moonCount}");
                Console.WriteLine($"{OutputString.AverageMoonTemperaturePrefix}{avgTempStr}");
                Console.WriteLine();
            }

            // Also render table view for structured presentation
            var columnSizes = new[] { 20, 20, 35 };
            var columnLabels = new[]
            {
                OutputString.PlanetNumber, OutputString.PlanetId, OutputString.PlanetMoonAverageTemperature
            };

            ConsoleWriter.CreateHeader(columnLabels, columnSizes);

            for (int i = 0, j = 1; i < planets.Length; i++, j++)
            {
                var avgTempDisplay = planets[i].AverageMoonTemperature.HasValue
                    ? $"{planets[i].AverageMoonTemperature.Value.ToString("F2", CultureInfo.InvariantCulture)} K"
                    : OutputString.NotAvailable;

                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(),
                        CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Id),
                        avgTempDisplay
                    },
                    columnSizes);
            }

            ConsoleWriter.CreateLine(columnSizes);
            ConsoleWriter.CreateEmptyLines(2);
        }
    }
}
