using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public class Container
{
    private Texture2D _lineTexture;
    private bool _renderEndLine;
    public Vector2 TopLeft { get; private set; }
    public Vector2 TopRight { get; private set; }
    public Vector2 BottomLeft { get; private set; }
    public Vector2 BottomRight { get; private set; }

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

    public void ShowEndLine()
    {
        _renderEndLine = true;
    }

    public void HideEndLine()
    {
        _renderEndLine = false;
    }

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

    public void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
    {
        var distance = Vector2.Distance(start, end);
        var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        spriteBatch.Draw(_lineTexture, start, null, color, angle, Vector2.Zero, new Vector2(distance, thickness),
            SpriteEffects.None, 0);
    }
}