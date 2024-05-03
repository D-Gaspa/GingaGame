using GingaGame_MonoGame.GameLogic;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame;

public class GameMode1Screen : GameModeScreenBase
{
    protected override GameMode Mode => GameMode.Mode1;

    public GameMode1Screen(Game1 game, Desktop desktop) : base(game, desktop)
    {
        // Game components are initialized in the base class
    }

    protected override void InitializeElements()
    {
        base.InitializeElements();

        Container.InitializeContainer(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Height,
            Game.GraphicsDevice.Viewport.Width, Mode);
    }
}