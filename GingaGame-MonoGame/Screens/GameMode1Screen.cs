using GingaGame_MonoGame.GameLogic;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame;

public class GameMode1Screen : GameModeScreenBase
{
    public GameMode1Screen(Game1 game, Desktop desktop) : base(game, desktop)
    {
        // Game components are initialized in the base class
    }

    protected override GameMode Mode => GameMode.Mode1;

    protected override void InitializeGameSpecificComponents()
    {
        // Game mode 1-specific code
        var planetMergingService = new PlanetMergingService(Scene, Mode, PlanetFactory, Score);
        var constraintHandler = new ConstraintHandler(Mode, Scene);
        CollisionManager = new CollisionManager(constraintHandler, Mode, GameStateHandler,
            planetMergingService, Scene);
    }

    protected override void InitializeElements()
    {
        base.InitializeElements();

        Container.InitializeContainer(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Height,
            Game.GraphicsDevice.Viewport.Width, Mode);
    }
}