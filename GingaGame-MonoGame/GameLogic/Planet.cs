using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     The Planet class represents a singular playable object within the game.
///     <para>
///         Each Planet has its own PlanetType along with other properties such as radius, points, texture, and collision
///         status.
///     </para>
/// </summary>
public class Planet : VerletPoint
{
    /// <summary>
    ///     Initializes new instance of the Planet class.
    /// </summary>
    /// <param name="planetType">The type of the planet to create.</param>
    /// <param name="position">The initial position of the planet.</param>
    public Planet(PlanetType planetType, Vector2 position)
        : base(position, PlanetData.FromPlanetType(planetType).Size)
    {
        PlanetType = planetType;

        var planetData = PlanetData.FromPlanetType(planetType);
        Radius = planetData.Size;
        Texture = planetData.Texture;
        Points = planetData.Points;
    }

    /// <summary>
    ///     Gets the type of the planet.
    /// </summary>
    public PlanetType PlanetType { get; }

    /// <summary>
    ///     Gets the radius of the planet.
    /// </summary>
    public float Radius { get; }

    /// <summary>
    ///     Gets the points awarded for the planet.
    /// </summary>
    public int Points { get; private set; }

    /// <summary>
    ///     Gets the texture used to represent the planet.
    /// </summary>
    private Texture2D Texture { get; }

    /// <summary>
    ///     Draws the planet with consideration for the Y-axis offset.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="yOffset"></param>
    public void Draw(SpriteBatch spriteBatch, float yOffset = 0)
    {
        var adjustedPosition = new Vector2(Position.X, Position.Y - yOffset);
        var imageWidth = Radius * 2;
        var imageHeight = Radius * 2;

        spriteBatch.Draw(Texture,
            new Rectangle((int)(adjustedPosition.X - imageWidth / 2), (int)(adjustedPosition.Y - imageHeight / 2),
                (int)imageWidth, (int)imageHeight), Color.White);
    }

    /// <summary>
    ///     Draw the planet at a specific size.
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="size"></param>
    public void DrawWithSize(SpriteBatch spriteBatch, float size)
    {
        var imageWidth = size * 2;
        var imageHeight = size * 2;

        spriteBatch.Draw(Texture,
            new Rectangle((int)(Position.X - imageWidth / 2), (int)(Position.Y - imageHeight / 2),
                (int)imageWidth, (int)imageHeight), Color.White);
    }
}