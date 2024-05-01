using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public class PlanetData
{
    private PlanetData(float size, int points, Texture2D texture)
    {
        Size = size;
        Points = points;
        Texture = texture;
    }

    public float Size { get; private set; }
    public int Points { get; private set; }
    public Texture2D Texture { get; private set; }

    public static PlanetData FromPlanetType(PlanetType planetType)
    {
        return new PlanetData(
            PlanetSizes.Sizes[(int)planetType],
            PlanetPoints.PointsPerPlanet[(int)planetType],
            PlanetTextures.GetCachedTexture(planetType)
        );
    }
}

public static class PlanetSizes
{
    public static Dictionary<int, float> Sizes { get; } = new()
    {
        { 0, 25f },
        { 1, 30f },
        { 2, 35f },
        { 3, 40f },
        { 4, 45f },
        { 5, 50f },
        { 6, 55f },
        { 7, 60f },
        { 8, 65f },
        { 9, 70f },
        { 10, 75f }
    };
}

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