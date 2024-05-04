using System;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Handles the game's state: checks end game conditions, updates and resets the game, and manages game pauses.
/// </summary>
public class GameStateHandler
{
    /// <summary>
    ///     The threshold for drawing the end line. If the planet is within this threshold, the end line will be drawn.
    /// </summary>
    private const int DrawEndLineThreshold = 70;

    /// <summary>
    ///     Tolerance is a constant value that determines the acceptable margin for the planet to be near the end line.
    /// </summary>
    private const int Tolerance = 5;

    private readonly Container _container;
    private readonly GameMode _gameMode;
    private readonly GameScreen _gameModeControl;
    private readonly Score _score;
    private readonly Scoreboard _scoreboard;
    private readonly UserInterfaceCreator _userInterfaceCreator;
    private bool _drawEndLine;
    private bool _gameOverTriggered;

    private bool _gameWonTriggered;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameStateHandler"/> class.
    /// </summary>
    /// <param name="container">The game container.</param>
    /// <param name="gameMode">The current game mode (GameMode.Mode1 or GameMode.Mode2).</param>
    /// <param name="gameModeControl">The GameScreen control.</param>
    /// <param name="score">The current score of the game.</param>
    /// <param name="scoreboard">The game's scoreboard.</param>
    /// <param name="userInterfaceCreator">The user interface creator.</param>
    public GameStateHandler(Container container, GameMode gameMode, GameScreen gameModeControl, Score score,
        Scoreboard scoreboard, UserInterfaceCreator userInterfaceCreator)
    {
        _container = container;
        _gameMode = gameMode;
        _gameModeControl = gameModeControl;
        _score = score;
        _scoreboard = scoreboard;
        _userInterfaceCreator = userInterfaceCreator;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether the game is over.
    /// </summary>
    public bool IsGameOver { get; set; }

    /// <summary>
    ///     Determines whether the game is currently paused or not.
    /// </summary>
    public bool IsGamePaused { get; private set; }

    /// <summary>
    ///     Updates the game state.
    /// </summary>
    public void Update()
    {
        if (!_drawEndLine)
            _container.HideEndLine();
        else
            _container.ShowEndLine();

        _drawEndLine = false;
    }

    /// <summary>
    ///     Checks if the game has reached the end conditions.
    /// </summary>
    /// <param name="planet">The planet to check end conditions for.</param>
    public void CheckGameEndConditions(Planet planet)
    {
        if (!_gameOverTriggered) CheckLoseCondition(planet);

        if (IsNearEndLine(planet) && _drawEndLine == false) _drawEndLine = true;
    }

    /// <summary>
    ///     Determines if a given planet is near the end line.
    /// </summary>
    /// <param name="planet">The planet to check.</param>
    /// <returns>True if the planet is near the end line; otherwise, false.</returns>
    private bool IsNearEndLine(Planet planet)
    {
        return planet.Position.Y < _container.TopLeft.Y + DrawEndLineThreshold + planet.Radius;
    }

    /// <summary>
    ///     Check the lose condition of the game.
    /// </summary>
    /// <param name="planet">The planet that is being checked.</param>
    private void CheckLoseCondition(Planet planet)
    {
        if (!(planet.Position.Y < _container.TopLeft.Y + planet.Radius - Tolerance)) return;

        _gameOverTriggered = true;

        // Check if the player won the game before losing
        if (_gameWonTriggered)
        {
            _userInterfaceCreator.ShowInputDialog("Game Over! Save your score!", "Enter your name:",
                (success, playerName) =>
                {
                    if (success) _scoreboard.AddScore(playerName, _score.CurrentScore);

                    ResetGame();
                });
            return;
        }

        _userInterfaceCreator.ShowMessageWindow("Game Over! You lost!");

        ResetGame();
    }

    /// <summary>
    ///     Checks the win condition based on the game mode and the given planet.
    /// </summary>
    /// <param name="planet">The planet to check the win condition for.</param>
    public void CheckWinCondition(Planet planet)
    {
        switch (_gameMode)
        {
            case GameMode.Mode1:
                if (planet.PlanetType != PlanetType.Sun || _gameWonTriggered) return;
                break;
            case GameMode.Mode2:
                if (planet.PlanetType != PlanetType.Pluto || _gameWonTriggered) return;
                break;
            default:
                throw new ArgumentException("Invalid game mode");
        }

        _gameWonTriggered = true;
        _userInterfaceCreator.ShowMessageWindow("Congratulations! You won!");
    }

    /// <summary>
    ///     Resets the game by setting necessary variables back to their initial state.
    /// </summary>
    private void ResetGame()
    {
        IsGameOver = true;
        _gameModeControl.ResetGame();
        _gameOverTriggered = false;
        _gameWonTriggered = false;
    }

    /// <summary>
    ///     Resumes the paused game.
    /// </summary>
    public void ResumeGame()
    {
        IsGamePaused = false;
    }

    /// <summary>
    ///     Pauses the game.
    /// </summary>
    public void PauseGame()
    {
        IsGamePaused = true;
    }
}