namespace Test_Taste_Console_Application.Domain.Services.Interfaces
{
    /// <summary>
    /// Service to export processed planetary and lunar data to CSV files.
    /// </summary>
    public interface IFileOutputService
    {
        /// <summary>
        /// Exports all planets and their moons to a CSV file.
        /// </summary>
        void OutputAllPlanetsAndTheirMoonsToFile();

        /// <summary>
        /// Exports all moons and their mass properties to a CSV file.
        /// </summary>
        void OutputAllMoonsAndTheirMassToFile();

        /// <summary>
        /// Exports all planets with moons and their average moon temperature to a CSV file.
        /// </summary>
        void OutputAllPlanetsAndTheirAverageMoonTemperatureToFile();

        /// <summary>
        /// Executes all file export operations.
        /// </summary>
        void OutputAllToFile();
    }
}
