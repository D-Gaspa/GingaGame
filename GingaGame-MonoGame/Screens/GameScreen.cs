using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame;

/// <summary>
///     Represents a screen in the game. This is an abstract base class for all game screens.
/// </summary>
public abstract class GameScreen
{
    protected readonly Game1 Game;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameScreen" /> class.
    /// </summary>
    /// <param name="game">A reference to the main Game instance.</param>
    protected GameScreen(Game1 game)
    {
        Game = game;
    }

    /// <summary>
    ///     Load the content for the screen.
    ///     Should be overridden by deriving classes to load their specific content.
    /// </summary>
    public abstract void LoadContent();

    /// <summary>
    ///     Updates the state of the screen according to the provided game time.
    /// </summary>
    /// <param name="gameTime">The current game time snapshot.</param>
    public abstract void Update(GameTime gameTime);

    /// <summary>
    ///     Draws the content of the screen.
    /// </summary>
    public abstract void Draw();

    /// <summary>
    ///     Resets the state of the game screen.
    /// </summary>
    public abstract void ResetGame();

    /// <summary>
    ///     Resumes the game logic after being paused.
    /// </summary>
    public abstract void ResumeGame();

    /// <summary>
    ///     Pause the game logic.
    /// </summary>
    public abstract void PauseGame();
}