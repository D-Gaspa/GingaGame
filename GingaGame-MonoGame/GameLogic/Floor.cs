using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public class Floor
{
    public int StartPositionY { get; set; }
    public int EndPositionY { get; set; }
    public int Index { get; set; }
    public int NextPlanetIndex { get; set; } // The index of the planet that the next level will allow

    public void Draw(SpriteBatch spriteBatch, Container container, float yOffset = 0)
    {
        // Adjust the Y position with the offset
        var adjustedEndPositionY = EndPositionY - yOffset;

        var isLastFloor = NextPlanetIndex < 0; // Check if the current floor is the last one

        // Set the color to red if it's the last floor, otherwise set it to white
        var rectangleColor = isLastFloor ? new Color(255, 0, 0, 50) : new Color(255, 255, 255, 75);

        const int rectangleHeight = 30; // The height of the rectangle
        const int planetRadius = 15; // The radius of the planet
        var rectangleY = adjustedEndPositionY - rectangleHeight; // The Y position of the rectangle

        DrawFloorRectangle(spriteBatch, container, rectangleColor, rectangleY, rectangleHeight);

        // If it's not the last floor, draw the planet with a fixed radius to the left of the rectangle
        if (isLastFloor) return;
        DrawNextFloorPlanet(spriteBatch, container, planetRadius, rectangleY);
    }

    private static void DrawFloorRectangle(SpriteBatch spriteBatch, Container container, Color rectangleColor,
        float rectangleY, int rectangleHeight)
    {
        // Calculate the four corners of the rectangle
        var topLeft = new Vector2(container.TopLeft.X, rectangleY);
        var topRight = new Vector2(container.BottomRight.X, rectangleY);
        var bottomRight = new Vector2(container.BottomRight.X, rectangleY + rectangleHeight);
        var bottomLeft = new Vector2(container.TopLeft.X, rectangleY + rectangleHeight);

        // Draw the four sides of the rectangle
        container.DrawLine(spriteBatch, topLeft, topRight, rectangleColor, 1f);
        container.DrawLine(spriteBatch, topRight, bottomRight, rectangleColor, 1f);
        container.DrawLine(spriteBatch, bottomRight, bottomLeft, rectangleColor, 1f);
        container.DrawLine(spriteBatch, bottomLeft, topLeft, rectangleColor, 1f);
    }

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