using System.Collections.Generic;
using System.Collections.ObjectModel;
using Test_Taste_Console_Application.Domain.DataTransferObjects;
using Test_Taste_Console_Application.Domain.Objects;

namespace Test_Taste_Console_Application.Domain.Data
{
    /// <summary>
    /// Provides standard Solar System reference data (from api.le-systeme-solaire.net)
    /// used when the remote API returns 401 Unauthorized or is unreachable.
    /// </summary>
    public static class SolarSystemSampleData
    {
        public static Collection<Planet> GetSamplePlanets()
        {
            var earthMoons = new Collection<MoonDto>
            {
                new MoonDto { Id = "lune", Moon = "La Lune", MassValue = 7.346f, MassExponent = 22, Gravity = 1.62f, AvgTemp = 250f }
            };

            var marsMoons = new Collection<MoonDto>
            {
                new MoonDto { Id = "phobos", Moon = "Phobos", MassValue = 1.0659f, MassExponent = 16, Gravity = 0.0057f, AvgTemp = 233f },
                new MoonDto { Id = "deimos", Moon = "Deïmos", MassValue = 1.4762f, MassExponent = 15, Gravity = 0.003f, AvgTemp = 233f }
            };

            var jupiterMoons = new Collection<MoonDto>
            {
                new MoonDto { Id = "io", Moon = "Io", MassValue = 8.9319f, MassExponent = 22, Gravity = 1.796f, AvgTemp = 130f },
                new MoonDto { Id = "europe", Moon = "Europe", MassValue = 4.8f, MassExponent = 22, Gravity = 1.315f, AvgTemp = 102f },
                new MoonDto { Id = "ganymede", Moon = "Ganymède", MassValue = 1.4819f, MassExponent = 23, Gravity = 1.428f, AvgTemp = 110f },
                new MoonDto { Id = "callisto", Moon = "Callisto", MassValue = 1.0759f, MassExponent = 23, Gravity = 1.235f, AvgTemp = 134f }
            };

            var saturnMoons = new Collection<MoonDto>
            {
                new MoonDto { Id = "mimas", Moon = "Mimas", MassValue = 3.75f, MassExponent = 19, Gravity = 0.064f, AvgTemp = 64f },
                new MoonDto { Id = "encelade", Moon = "Encelade", MassValue = 1.08f, MassExponent = 20, Gravity = 0.113f, AvgTemp = 75f },
                new MoonDto { Id = "tethys", Moon = "Téthys", MassValue = 6.17f, MassExponent = 20, Gravity = 0.145f, AvgTemp = 86f },
                new MoonDto { Id = "dione", Moon = "Dioné", MassValue = 1.095f, MassExponent = 21, Gravity = 0.231f, AvgTemp = 87f },
                new MoonDto { Id = "rhea", Moon = "Rhéa", MassValue = 2.306f, MassExponent = 21, Gravity = 0.264f, AvgTemp = 73f },
                new MoonDto { Id = "titan", Moon = "Titan", MassValue = 1.3452f, MassExponent = 23, Gravity = 1.352f, AvgTemp = 94f },
                new MoonDto { Id = "iapet", Moon = "Japet", MassValue = 1.805f, MassExponent = 21, Gravity = 0.223f, AvgTemp = 130f }
            };

            var uranusMoons = new Collection<MoonDto>
            {
                new MoonDto { Id = "ariel", Moon = "Ariel", MassValue = 1.353f, MassExponent = 21, Gravity = 0.269f, AvgTemp = 60f },
                new MoonDto { Id = "umbriel", Moon = "Umbriel", MassValue = 1.172f, MassExponent = 21, Gravity = 0.2f, AvgTemp = 75f },
                new MoonDto { Id = "titania", Moon = "Titania", MassValue = 3.527f, MassExponent = 21, Gravity = 0.378f, AvgTemp = 60f },
                new MoonDto { Id = "oberon", Moon = "Obéron", MassValue = 3.014f, MassExponent = 21, Gravity = 0.346f, AvgTemp = 61f },
                new MoonDto { Id = "miranda", Moon = "Miranda", MassValue = 6.59f, MassExponent = 19, Gravity = 0.079f, AvgTemp = 60f }
            };

            var neptuneMoons = new Collection<MoonDto>
            {
                new MoonDto { Id = "triton", Moon = "Triton", MassValue = 2.14f, MassExponent = 22, Gravity = 0.779f, AvgTemp = 38f },
                new MoonDto { Id = "nereide", Moon = "Néréide", MassValue = 3.1f, MassExponent = 19, Gravity = 0.071f, AvgTemp = 50f },
                new MoonDto { Id = "protee", Moon = "Protée", MassValue = 4.4f, MassExponent = 19, Gravity = 0.07f, AvgTemp = 50f }
            };

            return new Collection<Planet>
            {
                new Planet(new PlanetDto { Id = "mercure", SemiMajorAxis = 57909050f, Moons = new Collection<MoonDto>() }),
                new Planet(new PlanetDto { Id = "venus", SemiMajorAxis = 108208000f, Moons = new Collection<MoonDto>() }),
                new Planet(new PlanetDto { Id = "terre", SemiMajorAxis = 149598262f, Moons = earthMoons }),
                new Planet(new PlanetDto { Id = "mars", SemiMajorAxis = 227939200f, Moons = marsMoons }),
                new Planet(new PlanetDto { Id = "jupiter", SemiMajorAxis = 778570000f, Moons = jupiterMoons }),
                new Planet(new PlanetDto { Id = "saturne", SemiMajorAxis = 1433530000f, Moons = saturnMoons }),
                new Planet(new PlanetDto { Id = "uranus", SemiMajorAxis = 2872460000f, Moons = uranusMoons }),
                new Planet(new PlanetDto { Id = "neptune", SemiMajorAxis = 4495060000f, Moons = neptuneMoons })
            };
        }

        public static Collection<Moon> GetSampleMoons()
        {
            var planets = GetSamplePlanets();
            var moons = new Collection<Moon>();
            foreach (var planet in planets)
            {
                if (planet.Moons != null)
                {
                    foreach (var moon in planet.Moons)
                    {
                        moons.Add(moon);
                    }
                }
            }
            return moons;
        }
    }
}
