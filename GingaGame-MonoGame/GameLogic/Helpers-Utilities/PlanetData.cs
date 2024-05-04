using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Represents data associated with a planet, including its size, points, and texture.
/// </summary>
public class PlanetData
{
    /// <summary>
    ///     Constructor for the <see cref="PlanetData"/> class.
    /// </summary>
    private PlanetData(float size, int points, Texture2D texture)
    {
        Size = size;
        Points = points;
        Texture = texture;
    }

    /// <summary>
    ///     Gets the size of the planet.
    /// </summary>
    public float Size { get; private set; }

    /// <summary>
    ///     Gets the points associated with the planet.
    /// </summary>
    public int Points { get; private set; }

    /// <summary>
    ///     Gets the texture used to represent the planet.
    /// </summary>
    public Texture2D Texture { get; private set; }

    /// <summary>
    ///     Creates a <see cref="PlanetData"/> instance from the given PlanetType.
    /// </summary>
    /// <param name="planetType">Planet type whose data to generate.</param>
    /// <returns>Returns a PlanetData instance.</returns>
    public static PlanetData FromPlanetType(PlanetType planetType)
    {
        return new PlanetData(
            PlanetSizes.Sizes[(int)planetType],
            PlanetPoints.PointsPerPlanet[(int)planetType],
            PlanetTextures.GetCachedTexture(planetType)
        );
    }
}

/// <summary>
///     Enum representing the different types of Planets.
/// </summary>
public enum PlanetType
{
    Pluto,
    Moon,
    Mercury,
    Mars,
    Venus,
    Earth,
    Neptune,
    Uranus,
    Saturn,
    Jupiter,
    Sun
}

/// <summary>
///     Static dictionary holding the sizes of all PlanetTypes.
/// </summary>
public static class PlanetSizes
{
    public static Dictionary<int, float> Sizes { get; } = new()
    {
        { 0, 40 },
        { 1, 45 },
        { 2, 50 },
        { 3, 55 },
        { 4, 60 },
        { 5, 65 },
        { 6, 70 },
        { 7, 75 },
        { 8, 80 },
        { 9, 85 },
        { 10, 90 }
    };
}

/// <summary>
///     Static dictionary holding the points of all PlanetTypes.
/// </summary>
public static class PlanetPoints
{
    public static Dictionary<int, int> PointsPerPlanet { get; } = new()
    {
        { 0, 10 },
        { 1, 12 },
        { 2, 14 },
        { 3, 16 },
        { 4, 18 },
        { 5, 20 },
        { 6, 22 },
        { 7, 24 },
        { 8, 26 },
        { 9, 28 },
        { 10, 30 }
    };
}