using System;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Services;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            // The ConfigureServices function configures the dependency injection container.
            ConfigureServices(serviceCollection);

            // The RunServiceOperations function executes the operations that produce screen and file outputs.
            RunServiceOperations(serviceCollection);
        }

        private static void RunServiceOperations(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Retrieve services from DI container
            var screenOutputService = serviceProvider.GetService<IOutputService>();
            var fileOutputService = serviceProvider.GetService<IFileOutputService>();

            try
            {
                Console.WriteLine("Loading data from Solar System API...");
                Console.WriteLine("Processing planetary data...");
                Console.WriteLine();

                Console.WriteLine("Writing results to console...");
                Console.WriteLine();

                // 1. New Feature: Planets with moons and their average moon temperature
                screenOutputService.OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();

                // 2. Existing Feature: All planets and their moons
                screenOutputService.OutputAllPlanetsAndTheirMoonsToConsole();

                // 3. Existing Feature: All moons and their masses
                screenOutputService.OutputAllMoonsAndTheirMassToConsole();

                // 4. Existing Feature: All planets and their average moon gravity
                screenOutputService.OutputAllPlanetsAndTheirAverageMoonGravityToConsole();

                Console.WriteLine("Writing results to file...");

                // Write CSV output files to FileOutput folder
                fileOutputService.OutputAllToFile();

                Console.WriteLine();
                Console.WriteLine("Process completed successfully.");
            }
            catch (Exception exception)
            {
                // Log and notify of any unhandled exceptions
                Logger.Instance.Error($"{LoggerMessage.ScreenOutputOperationFailed}{exception.Message}");
                Console.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
                System.Diagnostics.Debug.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
            }
            finally
            {
                serviceProvider.Dispose();
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Configure log4net
            XmlConfigurator.Configure(
                LogManager.GetRepository(Assembly.GetEntryAssembly()),
                new FileInfo(ConfigurationFileName.Logger));

            // Register application services
            serviceCollection.AddHttpClient<HttpClientService>();
            serviceCollection.AddSingleton<IPlanetService, PlanetService>();
            serviceCollection.AddSingleton<IMoonService, MoonService>();
            serviceCollection.AddSingleton<IOutputService, ScreenOutputService>();
            serviceCollection.AddSingleton<IFileOutputService, FileOutputService>();
        }
    }
}
