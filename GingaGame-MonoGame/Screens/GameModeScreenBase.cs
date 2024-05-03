using System;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Container = GingaGame_MonoGame.GameLogic.Container;

namespace GingaGame_MonoGame;

public abstract class GameModeScreenBase : GameScreen
{
    protected const double ClickDelay = 0.5; // Delay in seconds
    protected CollisionManager CollisionManager;
    protected Container Container;
    protected Planet CurrentPlanet;
    protected double ElapsedTime;
    protected GameStateHandler GameStateHandler;
    protected bool IsInputEnabled = true;
    protected Planet NextPlanet;
    protected PlanetFactory PlanetFactory;
    protected Scene Scene;
    protected Score Score;
    protected Scoreboard Scoreboard;
    protected GameUserInterfaceManager UserInterfaceManager;

    protected GameModeScreenBase(Game1 game, Desktop desktop) : base(game)
    {
        InitializeGameComponents(game, desktop);
    }

    protected abstract GameMode Mode { get; }

    private bool IsGameOver => GameStateHandler.IsGameOver;
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
        Scoreboard = new Scoreboard(Mode);

        var userInterfaceCreator = new UserInterfaceCreator(desktop, this);
        GameStateHandler = new GameStateHandler(Container, Mode, this, Score, Scoreboard, userInterfaceCreator);

        var planetMergingService = new PlanetMergingService(Scene, Mode, PlanetFactory, Score);
        var constraintHandler = new ConstraintHandler(Mode, Scene);
        CollisionManager = new CollisionManager(constraintHandler, Mode, GameStateHandler,
            planetMergingService, Scene);

        SetupPlanetsAndScene();
    }

    public override void LoadContent()
    {
        UserInterfaceManager.LoadContent();
        InitializeElements();
    }

    protected virtual void InitializeElements()
    {
        UserInterfaceManager.Initialize(Scoreboard);
    }

    public override void ResetGame()
    {
        // Reset the game state and initialize the game again
        Scene.ClearPlanets();
        Score.ResetScore();
        PlanetFactory.InitializeDefaultPlanetByGameMode();
        ElapsedTime = 0;
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

        CurrentPlanet = new Planet(planetType, new Vector2(middleX, 0))
        {
            IsPinned = true
        };

        Scene.AddPlanet(CurrentPlanet);
        GenerateNextPlanet();
    }

    public override void ResumeGame()
    {
        IsInputEnabled = true;

        UserInterfaceManager.UpdateScoreboardText(Scoreboard);

        GameStateHandler.ResumeGame();
    }

    public override void PauseGame()
    {
        IsInputEnabled = false;
        GameStateHandler.PauseGame();
    }

    private void GenerateNextPlanet()
    {
        NextPlanet = PlanetFactory.GenerateNextPlanet(Game.GraphicsDevice.Viewport.Width);
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
        if (CurrentPlanet.IsPinned) UpdateCurrentPlanetPosition(mouseState);

        // Update the planet positions
        Scene.Update();

        // Check for collisions
        CollisionManager.RunCollisions(8);

        // Update the score if it has changed
        UpdateScoreIfChanged();

        GameStateHandler.Update();
    }

    private void HandleMouseClick(MouseState mouseState)
    {
        // Check if the left mouse button is pressed and input is enabled
        if (mouseState.LeftButton != ButtonState.Pressed || !IsInputEnabled || IsGameOver) return;
        if (!CurrentPlanet.IsPinned) return;

        UpdateCurrentPlanetPosition(mouseState);
        CurrentPlanet.IsPinned = false;

        // Disable input
        IsInputEnabled = false;
    }

    private void HandleElapsedTime(GameTime gameTime)
    {
        ElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        // If the elapsed time is greater than the delay, switch the planet and re-enable input
        if (!(ElapsedTime >= ClickDelay) || IsGameOver) return;
        SwitchPlanet();
        IsInputEnabled = true;
        ElapsedTime = 0;
    }

    private void SwitchPlanet()
    {
        // Switch the current planet with the next planet
        CurrentPlanet = NextPlanet;

        // Pin the planet
        CurrentPlanet.IsPinned = true;

        // Add the planet to the scene
        Scene.AddPlanet(CurrentPlanet);

        // Generate the next planet
        GenerateNextPlanet();
    }

    private void UpdateCurrentPlanetPosition(MouseState mouseState)
    {
        var x = mouseState.X;

        // Check if the planet is out of bounds
        // If it is, set the position to the closest edge of the container
        // Otherwise, set the position to the mouse's X coordinate
        if (x < Container.TopLeft.X + CurrentPlanet.Radius)
        {
            CurrentPlanet.Position.X = Container.TopLeft.X + CurrentPlanet.Radius;
            CurrentPlanet.OldPosition.X = Container.TopLeft.X + CurrentPlanet.Radius;
        }
        else if (x > Container.BottomRight.X - CurrentPlanet.Radius)
        {
            CurrentPlanet.Position.X = Container.BottomRight.X - CurrentPlanet.Radius;
            CurrentPlanet.OldPosition.X = Container.BottomRight.X - CurrentPlanet.Radius;
        }
        else
        {
            CurrentPlanet.Position.X = x;
            CurrentPlanet.OldPosition.X = x;
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

        // Draw the scene (planets, container and floors (if game mode 2)
        Scene.Draw(Game.SpriteBatch, Game.GraphicsDevice.Viewport.Height);

        // Draw the next planet
        UserInterfaceManager.DrawNextPlanet(NextPlanet);

        Game.SpriteBatch.End();
    }
}