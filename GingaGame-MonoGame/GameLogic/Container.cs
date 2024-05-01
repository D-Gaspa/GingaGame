using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public class Container
{
    private const int VerticalTopMargin = 70;
    private Texture2D _lineTexture;
    public Vector2 TopLeft { get; set; }
    public Vector2 TopRight { get; set; }
    public Vector2 BottomLeft { get; set; }
    private Vector2 BottomRight { get; set; }

    public void InitializeContainer(GraphicsDevice graphicsDevice, int displayWidth, int containerHeight)
    {
        _lineTexture = new Texture2D(graphicsDevice, 1, 1);
        _lineTexture.SetData(new[] { Color.White });

        const float verticalMargin = VerticalTopMargin;
        const float horizontalLength = 450;
        var horizontalMargin = (displayWidth - horizontalLength) / 2;

        TopLeft = new Vector2(horizontalMargin, verticalMargin);
        TopRight = new Vector2(displayWidth - horizontalMargin, verticalMargin);
        BottomLeft = new Vector2(horizontalMargin, containerHeight - verticalMargin);
        BottomRight = new Vector2(displayWidth - horizontalMargin, containerHeight - verticalMargin);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawLine(spriteBatch, TopRight, BottomRight, Color.White, 1f);
        DrawLine(spriteBatch, BottomRight, BottomLeft, Color.White, 1f);
        DrawLine(spriteBatch, BottomLeft, TopLeft, Color.White, 1f);
    }

    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float thickness)
    {
        var distance = Vector2.Distance(start, end);
        var angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        spriteBatch.Draw(_lineTexture, start, null, color, angle, Vector2.Zero, new Vector2(distance, thickness),
            SpriteEffects.None, 0);
    }
}