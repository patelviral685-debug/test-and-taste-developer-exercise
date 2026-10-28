using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class FileOutputService : IFileOutputService
    {
        private readonly IPlanetService _planetService;
        private readonly IMoonService _moonService;

        public FileOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        public void OutputAllToFile()
        {
            OutputAllPlanetsAndTheirAverageMoonTemperatureToFile();
            OutputAllPlanetsAndTheirMoonsToFile();
            OutputAllMoonsAndTheirMassToFile();
        }

        public void OutputAllPlanetsAndTheirAverageMoonTemperatureToFile()
        {
            try
            {
                EnsureOutputDirectoryExists();

                var planets = _planetService.GetAllPlanets()
                    .Where(p => p.HasMoons())
                    .ToArray();

                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirAverageMoonTemperatureFile);
                var sb = new StringBuilder();

                // CSV Header
                sb.AppendLine("Planet Number,Planet Id,Total Moons,Average Moon Temperature (K)");

                for (int i = 0, j = 1; i < planets.Length; i++, j++)
                {
                    var planet = planets[i];
                    var planetName = EscapeCsv(CultureInfoUtility.TextInfo.ToTitleCase(planet.Id));
                    var moonCount = planet.Moons.Count;
                    var avgTemp = planet.AverageMoonTemperature.HasValue
                        ? planet.AverageMoonTemperature.Value.ToString("F2", CultureInfo.InvariantCulture)
                        : OutputString.NotAvailable;

                    sb.AppendLine($"{j},{planetName},{moonCount},{avgTemp}");
                }

                File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception exception)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{exception.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{exception.Message}");
            }
        }

        public void OutputAllPlanetsAndTheirMoonsToFile()
        {
            try
            {
                EnsureOutputDirectoryExists();

                var planets = _planetService.GetAllPlanets().ToArray();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllPlanetsAndTheirMoonsFile);
                var sb = new StringBuilder();

                // CSV Header
                sb.AppendLine("Planet Number,Planet Id,Planet Semi-Major Axis,Total Moons,Moon Number,Moon Id");

                for (int i = 0, j = 1; i < planets.Length; i++, j++)
                {
                    var planet = planets[i];
                    var planetName = EscapeCsv(CultureInfoUtility.TextInfo.ToTitleCase(planet.Id));
                    var semiMajorAxis = planet.SemiMajorAxis.ToString(CultureInfo.InvariantCulture);
                    var moonCount = planet.Moons.Count;

                    if (planet.HasMoons())
                    {
                        for (int k = 0, l = 1; k < planet.Moons.Count; k++, l++)
                        {
                            var moon = planet.Moons.ElementAt(k);
                            var moonName = EscapeCsv(CultureInfoUtility.TextInfo.ToTitleCase(moon.Id));
                            sb.AppendLine($"{j},{planetName},{semiMajorAxis},{moonCount},{l},{moonName}");
                        }
                    }
                    else
                    {
                        sb.AppendLine($"{j},{planetName},{semiMajorAxis},{moonCount},-,-");
                    }
                }

                File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception exception)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{exception.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{exception.Message}");
            }
        }

        public void OutputAllMoonsAndTheirMassToFile()
        {
            try
            {
                EnsureOutputDirectoryExists();

                var moons = _moonService.GetAllMoons().ToArray();
                var filePath = Path.Combine(PathName.PathToOutputFolder, PathName.AllMoonsAndTheirMassFile);
                var sb = new StringBuilder();

                // CSV Header
                sb.AppendLine("Moon Number,Moon Id,Moon Mass Exponent,Moon Mass Value");

                for (int i = 0, j = 1; i < moons.Length; i++, j++)
                {
                    var moon = moons[i];
                    var moonName = EscapeCsv(CultureInfoUtility.TextInfo.ToTitleCase(moon.Id));
                    var massExponent = moon.MassExponent.ToString(CultureInfo.InvariantCulture);
                    var massValue = moon.MassValue.ToString(CultureInfo.InvariantCulture);

                    sb.AppendLine($"{j},{moonName},{massExponent},{massValue}");
                }

                File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
                Console.WriteLine($"{OutputString.FileCreated}{filePath}");
            }
            catch (Exception exception)
            {
                Logger.Instance.Error($"{LoggerMessage.FileOutputOperationFailed}{exception.Message}");
                Console.WriteLine($"{ExceptionMessage.FileOutputOperationFailed}{exception.Message}");
            }
        }

        private static void EnsureOutputDirectoryExists()
        {
            if (!Directory.Exists(PathName.PathToOutputFolder))
            {
                Directory.CreateDirectory(PathName.PathToOutputFolder);
                Console.WriteLine($"{OutputString.FolderCreated}{PathName.PathToOutputFolder}");
            }
        }

        private static string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n") || value.Contains("\r"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}
