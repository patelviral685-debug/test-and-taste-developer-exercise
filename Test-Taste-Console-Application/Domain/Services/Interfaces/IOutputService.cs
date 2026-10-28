namespace Test_Taste_Console_Application.Domain.Services.Interfaces
{
    /// <summary>
    /// An output service that displays Solar System data to the user via the console.
    /// </summary>
    public interface IOutputService
    {
        void OutputAllPlanetsAndTheirMoonsToConsole();
        void OutputAllMoonsAndTheirMassToConsole();
        void OutputAllPlanetsAndTheirAverageMoonGravityToConsole();

        /// <summary>
        /// Displays all planets that have at least one moon, along with their moon count
        /// and the average temperature of their moons.
        /// </summary>
        void OutputAllPlanetsAndTheirAverageMoonTemperatureToConsole();
    }
}
