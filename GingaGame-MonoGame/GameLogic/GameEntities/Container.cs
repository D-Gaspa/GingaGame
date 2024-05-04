using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Defines the boundaries of the game container and provides methods to draw the bounds.
/// </summary>
public class Container
{
    private Texture2D _lineTexture;
    private bool _renderEndLine;
    public Vector2 TopLeft { get; private set; }
    public Vector2 TopRight { get; private set; }
    public Vector2 BottomLeft { get; private set; }
    public Vector2 BottomRight { get; private set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Container"/> class with the specified game mode, size, and margin settings.
    /// </summary>
    /// <param name="graphicsDevice">A GraphicsDevice that represents the display device.</param>
    /// <param name="containerHeight">Height of the container.</param>
    /// <param name="displayWidth">Screen width.</param>
    /// <param name="gameMode">The current game mode (GameMode.Mode1 or GameMode.Mode2).</param>
    /// <param name="verticalTopMargin">Top margin for the container.</param>
    /// <param name="verticalBottomMargin">Bottom margin for the container.</param>
    /// <param name="horizontalMargin">Horizontal margin for the container.</param>
    public void InitializeContainer(GraphicsDevice graphicsDevice, int containerHeight, int displayWidth,
        GameMode gameMode, int verticalTopMargin = 120, int verticalBottomMargin = 70, float horizontalMargin = 0)
    {
        _lineTexture = new Texture2D(graphicsDevice, 1, 1);
        _lineTexture.SetData(new[] { Color.White });

        if (horizontalMargin <= 0)
        {
            const float horizontalLength = 450;
            horizontalMargin = (displayWidth - horizontalLength) / 2;
        }

        TopLeft = new Vector2(horizontalMargin, verticalTopMargin);
        TopRight = new Vector2(displayWidth - horizontalMargin, verticalTopMargin);

        switch (gameMode)
        {
            case GameMode.Mode1:
                BottomLeft = new Vector2(horizontalMargin, containerHeight - verticalBottomMargin);
                BottomRight = new Vector2(displayWidth - horizontalMargin, containerHeight - verticalBottomMargin);
                break;
            case GameMode.Mode2:
                BottomLeft = new Vector2(horizontalMargin, containerHeight);
                BottomRight = new Vector2(displayWidth - horizontalMargin, containerHeight);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameMode), gameMode, null);
        }
    }

    /// <summary>
    ///     Reveals the end line of the container.
    /// </summary>
    public void ShowEndLine()
    {
        _renderEndLine = true;
    }

    /// <summary>
    ///     Hides the end line of the container.
    /// </summary>
    public void HideEndLine()
    {
        _renderEndLine = false;
    }

    /// <summary>
    ///     Draws the container on the given spriteBatch.
    /// </summary>
    /// <param name="spriteBatch">A batch used for drawing a series of sprites.</param>
    /// <param name="yOffset">
    ///     The vertical offset to adjust the container by, in case it needs to be moved on the vertical
    ///     axis.
    /// </param>
    public void Draw(SpriteBatch spriteBatch, float yOffset = 0)
    {
        // Adjust the Y position with the offset (if any)
        var adjustedTopRight = TopRight with { Y = TopRight.Y - yOffset };
        var adjustedBottomRight = BottomRight with { Y = BottomRight.Y - yOffset };
        var adjustedBottomLeft = BottomLeft with { Y = BottomLeft.Y - yOffset };
        var adjustedTopLeft = TopLeft with { Y = TopLeft.Y - yOffset };

        DrawLine(spriteBatch, adjustedTopRight, adjustedBottomRight, Color.White, 1f);
        DrawLine(spriteBatch, adjustedBottomRight, adjustedBottomLeft, Color.White, 1f);
        DrawLine(spriteBatch, adjustedBottomLeft, adjustedTopLeft, Color.White, 1f);

        if (_renderEndLine) DrawLine(spriteBatch, adjustedTopRight, adjustedTopLeft, Color.Red, 1f);
    }

    /// <summary>
    ///     Draws a line on the given spriteBatch.
    /// </summary>
    /// <param name="spriteBatch">A batch used for drawing a series of sprites.</param>
    /// <param name="start">The starting point of this line.</param>
    /// <param name="end">The ending point of this line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    public void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
    {
        var distance = Vector2.Distance(start, end);
        var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        spriteBatch.Draw(_lineTexture, start, null, color, angle, Vector2.Zero, new Vector2(distance, thickness),
            SpriteEffects.None, 0);
    }
}