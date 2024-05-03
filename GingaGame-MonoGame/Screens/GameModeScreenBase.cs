using System;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Container = GingaGame_MonoGame.GameLogic.Container;

namespace GingaGame_MonoGame;

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
    protected bool MouseClickHandled;
    protected PlanetFactory PlanetFactory;
    protected Scene Scene;
    protected Score Score;
    protected GameUserInterfaceManager UserInterfaceManager;

    protected GameModeScreenBase(Game1 game, Desktop desktop) : base(game)
    {
        InitializeGameComponents(game, desktop);
    }

    protected abstract GameMode Mode { get; }

    protected bool IsGameOver => GameStateHandler.IsGameOver;
    private bool IsGamePaused => GameStateHandler.IsGamePaused;

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

    protected abstract void InitializeGameSpecificComponents();

    public override void LoadContent()
    {
        UserInterfaceManager.LoadContent();
        InitializeElements();
    }

    protected virtual void InitializeElements()
    {
        UserInterfaceManager.Initialize(_scoreboard);
    }

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

    public override void ResumeGame()
    {
        IsInputEnabled = true;

        UserInterfaceManager.UpdateScoreboardText(_scoreboard);

        GameStateHandler.ResumeGame();
    }

    public override void PauseGame()
    {
        IsInputEnabled = false;
        GameStateHandler.PauseGame();
    }

    private void GenerateNextPlanet()
    {
        _nextPlanet = PlanetFactory.GenerateNextPlanet(Game.GraphicsDevice.Viewport.Width);
    }

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

    protected virtual void HandleMouseClick(MouseState mouseState)
    {
        // Check if the left mouse button is pressed and input is enabled
        if (mouseState.LeftButton != ButtonState.Pressed || !IsInputEnabled || IsGameOver) return;
        if (!CurrentPlanetToDrop.IsPinned) return;

        UpdateCurrentPlanetPosition(mouseState);
        CurrentPlanetToDrop.IsPinned = false;

        // Disable input
        IsInputEnabled = false;
    }

    private void HandleElapsedTime(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        // If the elapsed time is greater than the delay, switch the planet and re-enable input
        if (!(_elapsedTime >= ClickDelay) || IsGameOver) return;
        SwitchPlanet();
        IsInputEnabled = true;
        _elapsedTime = 0;
    }

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

    private void UpdateScoreIfChanged()
    {
        if (!Score.HasChanged) return;
        UserInterfaceManager.ScoreText = Score.CurrentScore.ToString();
        Score.HasChanged = false;
    }

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

    protected virtual void DrawScene()
    {
        Scene.Draw(Game.SpriteBatch, Game.GraphicsDevice.Viewport.Height);
    }
}