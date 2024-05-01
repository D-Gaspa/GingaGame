using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame;

public abstract class GameScreen
{
    protected readonly Game1 Game;

    protected GameScreen(Game1 game)
    {
        Game = game;
    }

    public abstract void LoadContent();
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(GameTime gameTime);
}