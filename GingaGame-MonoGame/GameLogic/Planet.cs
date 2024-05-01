using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public class Planet : VerletPoint
{
    public Planet(PlanetType planetType, Vector2 position, bool hasCollided = false)
        : base(position, PlanetData.FromPlanetType(planetType).Size)
    {
        PlanetType = planetType;

        var planetData = PlanetData.FromPlanetType(planetType);
        Radius = planetData.Size;
        Texture = planetData.Texture;
        Points = planetData.Points;
        HasCollided = hasCollided;
    }

    public PlanetType PlanetType { get; }
    public float Radius { get; }
    public int Points { get; private set; }
    public bool HasCollided { get; set; }
    public Texture2D Texture { get; }

    public void Draw(SpriteBatch spriteBatch, float yOffset = 0)
    {
        var adjustedPosition = new Vector2(Position.X, Position.Y - yOffset);
        var imageWidth = Radius * 2;
        var imageHeight = Radius * 2;

        spriteBatch.Draw(Texture,
            new Rectangle((int)(adjustedPosition.X - imageWidth / 2), (int)(adjustedPosition.Y - imageHeight / 2),
                (int)imageWidth, (int)imageHeight), Color.White);
    }
}