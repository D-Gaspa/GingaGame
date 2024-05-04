using GingaGame_MonoGame.GameLogic;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame;

/// <summary>
///     Represents a screen for game mode 1.
/// </summary>
public class GameMode1Screen : GameModeScreenBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GameMode1Screen" /> class.
    /// </summary>
    public GameMode1Screen(Game1 game, Desktop desktop) : base(game, desktop)
    {
        // Game components are initialized in the base class
    }

    /// <summary>
    ///     Represents the game mode of the screen. Value is set to the first game mode.
    /// </summary>
    protected override GameMode Mode => GameMode.Mode1;

    /// <summary>
    ///     Initializes the game-specific components of the game mode 1.
    /// </summary>
    protected override void InitializeGameSpecificComponents()
    {
        // Game mode 1-specific code
        var planetMergingService = new PlanetMergingService(Scene, Mode, PlanetFactory, Score);
        var constraintHandler = new ConstraintHandler(Mode, Scene);
        CollisionManager = new CollisionManager(constraintHandler, Mode, GameStateHandler,
            planetMergingService, Scene);
    }

    /// <summary>
    ///     Initializes the elements required for the game mode 1 screen.
    /// </summary>
    protected override void InitializeElements()
    {
        base.InitializeElements();

        Container.InitializeContainer(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Height,
            Game.GraphicsDevice.Viewport.Width, Mode);
    }
}