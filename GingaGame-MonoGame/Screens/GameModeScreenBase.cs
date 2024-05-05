using System;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Container = GingaGame_MonoGame.GameLogic.Container;

namespace GingaGame_MonoGame;

/// <summary>
///     The base class for game modes screens in the game.
/// </summary>
public abstract class GameModeScreenBase : GameScreen
{
    private const double ClickDelay = 0.5; // Delay in seconds
    private double _elapsedTime;
    private Planet _nextPlanet;
    private Scoreboard _scoreboard;
    protected CollisionManager CollisionManager;
    protected Container Container;
    protected Planet CurrentPlanetToDrop;
    protected GameStateHandler GameStateHandler;
    protected bool IsInputEnabled = true;
    protected PlanetFactory PlanetFactory;
    protected Scene Scene;
    protected Score Score;
    protected GameUserInterfaceManager UserInterfaceManager;

    /// <summary>
    ///     Initializes a new instance of the GameModeScreenBase class.
    /// </summary>
    /// <param name="game">A reference to the main Game instance.</param>
    /// <param name="desktop">The desktop UI manager.</param>
    protected GameModeScreenBase(Game1 game, Desktop desktop) : base(game)
    {
        InitializeGameComponents(game, desktop);
    }

    /// <summary>
    ///     Gets the game mode, should be overridden in derived classes
    /// </summary>
    protected abstract GameMode Mode { get; }

    /// <summary>
    ///     Gets a value indicating whether the game is over.
    /// </summary>
    protected bool IsGameOver => GameStateHandler.IsGameOver;

    /// <summary>
    ///     Gets a value indicating whether the game is paused.
    /// </summary>
    private bool IsGamePaused => GameStateHandler.IsGamePaused;

    /// <summary>
    ///     Initializes the common game components of a game mode.
    /// </summary>
    /// <param name="game">A reference to the main Game instance.</param>
    /// <param name="desktop">The desktop UI manager.</param>
    private void InitializeGameComponents(Game1 game, Desktop desktop)
    {
        PlanetTextures.SetContentManager(Game.Content);

        PlanetTextures.InitializePlanetTextures();

        UserInterfaceManager = new GameUserInterfaceManager(game, Mode);

        Container = new Container();

        Scene = new Scene(Container);

        PlanetFactory = new PlanetFactory(Mode);

        Score = new Score();
        _scoreboard = new Scoreboard(Mode);

        var userInterfaceCreator = new UserInterfaceCreator(desktop, this);
        GameStateHandler = new GameStateHandler(Container, Mode, this, Score, _scoreboard, userInterfaceCreator);

        InitializeGameSpecificComponents();

        SetupPlanetsAndScene();
    }

    /// <summary>
    ///     Initializes the game-specific components of the game mode.
    /// </summary>
    protected abstract void InitializeGameSpecificComponents();

    /// <summary>
    ///     Load the visual and non-visual content for the concrete game mode.
    /// </summary>
    public override void LoadContent()
    {
        UserInterfaceManager.LoadContent();
        InitializeElements();
    }

    /// <summary>
    ///     Initializes the elements required for the game mode screen.
    /// </summary>
    protected virtual void InitializeElements()
    {
        UserInterfaceManager.Initialize(_scoreboard);
    }

    /// <summary>
    ///     Resets the state of the game screen.
    /// </summary>
    public override void ResetGame()
    {
        // Reset the game state and initialize the game again
        Scene.ClearPlanets();
        Score.ResetScore();
        PlanetFactory.InitializeDefaultPlanetByGameMode();
        _elapsedTime = 0;
        SetupPlanetsAndScene();
        GameStateHandler.IsGameOver = false;
    }

    /// <summary>
    ///     Sets up the initial planets and scene for the game depending on the game mode.
    /// </summary>
    private void SetupPlanetsAndScene()
    {
        var middleX = Game.GraphicsDevice.Viewport.Width / 2;

        var planetType = Mode switch
        {
            GameMode.Mode1 => PlanetType.Pluto,
            GameMode.Mode2 => PlanetType.Sun,
            _ => throw new ArgumentOutOfRangeException()
        };

        CurrentPlanetToDrop = new Planet(planetType, new Vector2(middleX, 0))
        {
            IsPinned = true
        };

        Scene.AddPlanet(CurrentPlanetToDrop);
        GenerateNextPlanet();
    }

    /// <summary>
    ///     Resumes the game logic after being paused.
    /// </summary>
    public override void ResumeGame()
    {
        IsInputEnabled = true;

        UserInterfaceManager.UpdateScoreboardText(_scoreboard);

        GameStateHandler.ResumeGame();
    }

    /// <summary>
    ///     Pauses the game logic.
    /// </summary>
    public override void PauseGame()
    {
        IsInputEnabled = false;
        GameStateHandler.PauseGame();
    }

    /// <summary>
    ///     Generates the next planet to be dropped in the game.
    /// </summary>
    private void GenerateNextPlanet()
    {
        _nextPlanet = PlanetFactory.GenerateNextPlanet(Game.GraphicsDevice.Viewport.Width);
    }

    /// <summary>
    ///     Updates the game state.
    /// </summary>
    /// <param name="gameTime">The current game time.</param>
    public override void Update(GameTime gameTime)
    {
        // Get the current mouse state
        var mouseState = Mouse.GetState();

        // Handle mouse click
        HandleMouseClick(mouseState);

        // If input is disabled (and the game is not paused), increment the elapsed time
        if (!IsInputEnabled && !IsGamePaused) HandleElapsedTime(gameTime);

        // Check if the current planet is pinned
        if (CurrentPlanetToDrop.IsPinned) UpdateCurrentPlanetPosition(mouseState);

        // Update the planet positions
        Scene.Update();

        // Check for collisions
        CollisionManager.RunCollisions(8);

        // Update the score if it has changed
        UpdateScoreIfChanged();

        GameStateHandler.Update();
    }

    /// <summary>
    ///     Handles a mouse click event.
    /// </summary>
    /// <param name="mouseState">The current state of the mouse.</param>
    protected virtual void HandleMouseClick(MouseState mouseState)
    {
        // Check if the game window is active
        if (!Game.IsActive)
            return;
        
        // Check if the left mouse button is pressed and input is enabled
        if (mouseState.LeftButton != ButtonState.Pressed || !IsInputEnabled || IsGameOver) return;
        if (!CurrentPlanetToDrop.IsPinned) return;

        UpdateCurrentPlanetPosition(mouseState);
        CurrentPlanetToDrop.IsPinned = false;

        // Disable input
        IsInputEnabled = false;
    }

    /// <summary>
    ///     Handles the elapsed time in the game.
    /// </summary>
    /// <param name="gameTime">The game time.</param>
    private void HandleElapsedTime(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        // If the elapsed time is greater than the delay, switch the planet and re-enable input
        if (!(_elapsedTime >= ClickDelay) || IsGameOver) return;
        SwitchPlanet();
        IsInputEnabled = true;
        _elapsedTime = 0;
    }

    /// <summary>
    ///     Switches the current planet with the next planet, pinning the new planet and adding it to the scene.
    /// </summary>
    private void SwitchPlanet()
    {
        // Switch the current planet with the next planet
        CurrentPlanetToDrop = _nextPlanet;

        // Pin the planet
        CurrentPlanetToDrop.IsPinned = true;

        // Add the planet to the scene
        Scene.AddPlanet(CurrentPlanetToDrop);

        // Generate the next planet
        GenerateNextPlanet();
    }

    /// <summary>
    ///     Updates the current planet's position based on the mouse state.
    /// </summary>
    /// <param name="mouseState">The current state of the mouse.</param>
    protected void UpdateCurrentPlanetPosition(MouseState mouseState)
    {
        var x = mouseState.X;

        // Check if the planet is out of bounds
        // If it is, set the position to the closest edge of the container
        // Otherwise, set the position to the mouse's X coordinate
        if (x < Container.TopLeft.X + CurrentPlanetToDrop.Radius)
        {
            CurrentPlanetToDrop.Position.X = Container.TopLeft.X + CurrentPlanetToDrop.Radius;
            CurrentPlanetToDrop.OldPosition.X = Container.TopLeft.X + CurrentPlanetToDrop.Radius;
        }
        else if (x > Container.BottomRight.X - CurrentPlanetToDrop.Radius)
        {
            CurrentPlanetToDrop.Position.X = Container.BottomRight.X - CurrentPlanetToDrop.Radius;
            CurrentPlanetToDrop.OldPosition.X = Container.BottomRight.X - CurrentPlanetToDrop.Radius;
        }
        else
        {
            CurrentPlanetToDrop.Position.X = x;
            CurrentPlanetToDrop.OldPosition.X = x;
        }
    }

    /// <summary>
    ///     Updates the score if it has changed.
    /// </summary>
    private void UpdateScoreIfChanged()
    {
        if (!Score.HasChanged) return;
        UserInterfaceManager.ScoreText = Score.CurrentScore.ToString();
        Score.HasChanged = false;
    }

    /// <summary>
    ///     Draws the content of the screen.
    /// </summary>
    public override void Draw()
    {
        Game.SpriteBatch.Begin();

        // Draw the user interface elements
        UserInterfaceManager.DrawInterfaceElements();

        // Draw the scene
        DrawScene();

        // Draw the next planet
        UserInterfaceManager.DrawNextPlanet(_nextPlanet);

        Game.SpriteBatch.End();
    }

    /// <summary>
    ///     Draws the scene to the screen.
    /// </summary>
    /// <remarks>
    ///     This method is responsible for drawing the scene, including the planets, the container and floors (if any), to the
    ///     screen.
    /// </remarks>
    protected virtual void DrawScene()
    {
        Scene.Draw(Game.SpriteBatch, Game.GraphicsDevice.Viewport.Height);
    }
}