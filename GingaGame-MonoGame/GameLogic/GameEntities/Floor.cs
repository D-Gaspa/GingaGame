using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Represents a floor/level in the second game mode.
/// </summary>
public class Floor
{
    /// <summary>
    ///     Gets the Y position where the floor starts.
    /// </summary>
    public int StartPositionY { get; init; }

    /// <summary>
    ///     Gets the Y position where the floor ends.
    /// </summary>
    public int EndPositionY { get; init; }

    /// <summary>
    ///     Gets the index of the floor.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    ///     Gets the index of the planet that the next level will allow
    /// </summary>
    public int NextPlanetIndex { get; init; }

    /// <summary>
    ///     Draws the floor on the given spriteBatch.
    /// </summary>
    /// <param name="spriteBatch">A batch used for drawing a series of sprites.</param>
    /// <param name="container">The container in which the floor is drawn.</param>
    /// <param name="yOffset">The vertical offset to adjust the floor by, in case it needs to be moved on the vertical axis.</param>
    public void Draw(SpriteBatch spriteBatch, Container container, float yOffset = 0)
    {
        // Adjust the Y position with the offset
        var adjustedEndPositionY = EndPositionY - yOffset;

        var isLastFloor = NextPlanetIndex < 0; // Check if the current floor is the last one

        // Set the color to red if it's the last floor, otherwise set it to white
        var rectangleColor = isLastFloor ? new Color(255, 0, 0, 50) : new Color(255, 255, 255, 75);

        const int rectangleHeight = 50; // The height of the rectangle
        const int planetRadius = 25; // The radius of the planet
        var rectangleY = adjustedEndPositionY - rectangleHeight; // The Y position of the rectangle

        DrawFloorRectangle(spriteBatch, container, rectangleColor, rectangleY, rectangleHeight);

        // If it's not the last floor, draw the planet with a fixed radius to the left of the rectangle
        if (isLastFloor) return;
        DrawNextFloorPlanet(spriteBatch, container, planetRadius, rectangleY);
    }

    /// <summary>
    ///     Draws a rectangle that represents the floor on the SpriteBatch.
    /// </summary>
    /// <param name="spriteBatch">A batch used for drawing a series of sprites.</param>
    /// <param name="container">The container object used for getting boundaries.</param>
    /// <param name="rectangleColor">The color for the Backdrop rectangle.</param>
    /// <param name="rectangleY">Top Y coordinate for the rectangle.</param>
    /// <param name="rectangleHeight">Height of the rectangle.</param>
    private static void DrawFloorRectangle(SpriteBatch spriteBatch, Container container, Color rectangleColor,
        float rectangleY, int rectangleHeight)
    {
        // Calculate the rectangle's position and size
        var rectanglePosition = new Vector2(container.TopLeft.X, rectangleY);
        var rectangleSize = new Vector2(container.BottomRight.X - container.TopLeft.X, rectangleHeight);

        // Calculate the four corners of the rectangle
        var topLeft = new Vector2(container.TopLeft.X, rectangleY);
        var topRight = new Vector2(container.BottomRight.X, rectangleY);
        var bottomRight = new Vector2(container.BottomRight.X, rectangleY + rectangleHeight);
        var bottomLeft = new Vector2(container.TopLeft.X, rectangleY + rectangleHeight);

        // Create a 1x1 white pixel texture
        var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);

        pixel.SetData(new[] { Color.White });

        // Draw the rectangle with 40% opacity
        spriteBatch.Draw(pixel, rectanglePosition, null, rectangleColor * 0.4f, 0, Vector2.Zero, rectangleSize,
            SpriteEffects.None, 0);

        // Draw the outline of the rectangle
        container.DrawLine(spriteBatch, topLeft, topRight, rectangleColor, 1f);
        container.DrawLine(spriteBatch, topRight, bottomRight, rectangleColor, 1f);
        container.DrawLine(spriteBatch, bottomRight, bottomLeft, rectangleColor, 1f);
        container.DrawLine(spriteBatch, bottomLeft, topLeft, rectangleColor, 1f);
    }

    /// <summary>
    ///     Draws the planet image that will be at the next floor on the sprite batch.
    /// </summary>
    /// <param name="spriteBatch">A batch used for drawing a series of sprites.</param>
    /// <param name="container">The container object used for getting boundaries.</param>
    /// <param name="planetRadius">Radius of the planet image to be drawn.</param>
    /// <param name="rectangleY">Top Y coordinate for the start point of the planet image.</param>
    private void DrawNextFloorPlanet(SpriteBatch spriteBatch, Container container,
        int planetRadius, float rectangleY)
    {
        var planetX = container.TopLeft.X - planetRadius * 2;
        var planetY = rectangleY + planetRadius;

        // Draw the planet
        var planetType = (PlanetType)NextPlanetIndex; // Get the planet type from the next planet index

        var planet = new Planet(planetType, new Vector2(planetX, planetY));
        planet.DrawWithSize(spriteBatch, planetRadius);
    }
}