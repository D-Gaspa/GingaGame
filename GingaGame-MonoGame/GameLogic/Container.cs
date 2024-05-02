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

    public void InitializeContainer(GraphicsDevice graphicsDevice, int displayWidth, int containerHeight,
        int verticalTopMargin = 120, int verticalBottomMargin = 70)
    {
        _lineTexture = new Texture2D(graphicsDevice, 1, 1);
        _lineTexture.SetData(new[] { Color.White });

        const float horizontalLength = 450;
        var horizontalMargin = (displayWidth - horizontalLength) / 2;

        TopLeft = new Vector2(horizontalMargin, verticalTopMargin);
        TopRight = new Vector2(displayWidth - horizontalMargin, verticalTopMargin);
        BottomLeft = new Vector2(horizontalMargin, containerHeight - verticalBottomMargin);
        BottomRight = new Vector2(displayWidth - horizontalMargin, containerHeight - verticalBottomMargin);
    }

    public void ShowEndLine()
    {
        _renderEndLine = true;
    }

    public void HideEndLine()
    {
        _renderEndLine = false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawLine(spriteBatch, TopRight, BottomRight, Color.White, 1f);
        DrawLine(spriteBatch, BottomRight, BottomLeft, Color.White, 1f);
        DrawLine(spriteBatch, BottomLeft, TopLeft, Color.White, 1f);

        if (_renderEndLine) DrawLine(spriteBatch, TopRight, TopLeft, Color.Red, 1f);
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
    {
        var distance = Vector2.Distance(start, end);
        var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        spriteBatch.Draw(_lineTexture, start, null, color, angle, Vector2.Zero, new Vector2(distance, thickness),
            SpriteEffects.None, 0);
    }
}