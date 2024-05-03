using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Container = GingaGame_MonoGame.GameLogic.Container;

namespace GingaGame_MonoGame;

public class GameMode1Screen : GameScreen
{
    private const GameMode Mode = GameMode.Mode1;
    private const double ClickDelay = 0.5; // Delay in seconds
    private readonly CollisionManager _collisionManager;
    private readonly Container _container;
    private readonly GameStateHandler _gameStateHandler;
    private readonly PlanetFactory _planetFactory;
    private readonly Scene _scene;
    private readonly Score _score;
    private readonly Scoreboard _scoreboard;
    private readonly GameUserInterfaceManager _userInterfaceManager;
    private Planet _currentPlanet;
    private double _elapsedTime;
    private bool _isInputEnabled = true;
    private Planet _nextPlanet;

    public GameMode1Screen(Game1 game, Desktop desktop) : base(game)
    {
        // Set the content manager for the PlanetTextures class
        PlanetTextures.SetContentManager(Game.Content);

        PlanetTextures.InitializePlanetTextures();

        _userInterfaceManager = new GameUserInterfaceManager(game, Mode);

        _container = new Container();

        _scene = new Scene(_container);

        _planetFactory = new PlanetFactory(Mode);

        _score = new Score();
        _scoreboard = new Scoreboard(Mode);

        var userInterfaceCreator = new UserInterfaceCreator(desktop, this);
        _gameStateHandler = new GameStateHandler(_container, Mode, this, _score, _scoreboard, userInterfaceCreator);

        var planetMergingService = new PlanetMergingService(_scene, Mode, _planetFactory, _score);
        var constraintHandler = new ConstraintHandler(Mode, _scene);
        _collisionManager = new CollisionManager(constraintHandler, Mode, _gameStateHandler,
            planetMergingService, _scene);

        SetupPlanetsAndScene();
    }

    private bool IsGameOver => _gameStateHandler.IsGameOver;
    private bool IsGamePaused => _gameStateHandler.IsGamePaused;

    public override void LoadContent()
    {
        _userInterfaceManager.LoadContent();
        InitializeElements();
    }

    private void InitializeElements()
    {
        _userInterfaceManager.Initialize(_scoreboard);

        _container.InitializeContainer(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Height,
            Game.GraphicsDevice.Viewport.Width, Mode);
    }

    public override void ResetGame()
    {
        // Reset the game state and initialize the game again
        _scene.ClearPlanets();
        _score.ResetScore();
        _planetFactory.InitializeDefaultPlanetByGameMode();
        _elapsedTime = 0;
        SetupPlanetsAndScene();
        _gameStateHandler.IsGameOver = false;
    }

    private void SetupPlanetsAndScene()
    {
        var middleX = Game.GraphicsDevice.Viewport.Width / 2;
        _currentPlanet = new Planet(PlanetType.Pluto, new Vector2(middleX, 0))
        {
            IsPinned = true
        };
        _scene.AddPlanet(_currentPlanet);
        GenerateNextPlanet();
    }

    public override void ResumeGame()
    {
        _isInputEnabled = true;

        _userInterfaceManager.UpdateScoreboardText(_scoreboard);

        _gameStateHandler.ResumeGame();
    }

    public override void PauseGame()
    {
        _isInputEnabled = false;
        _gameStateHandler.PauseGame();
    }

    private void GenerateNextPlanet()
    {
        _nextPlanet = _planetFactory.GenerateNextPlanet(Game.GraphicsDevice.Viewport.Width);
    }

    public override void Update(GameTime gameTime)
    {
        // Get the current mouse state
        var mouseState = Mouse.GetState();

        // Handle mouse click
        HandleMouseClick(mouseState);

        // If input is disabled (and the game is not paused), increment the elapsed time
        if (!_isInputEnabled && !IsGamePaused) HandleElapsedTime(gameTime);

        // Check if the current planet is pinned
        if (_currentPlanet.IsPinned) UpdateCurrentPlanetPosition(mouseState);

        // Update the planet positions
        _scene.Update();

        // Check for collisions
        _collisionManager.RunCollisions(8);

        // Update the score if it has changed
        UpdateScoreIfChanged();

        _gameStateHandler.Update();
    }

    private void HandleMouseClick(MouseState mouseState)
    {
        // Check if the left mouse button is pressed and input is enabled
        if (mouseState.LeftButton != ButtonState.Pressed || !_isInputEnabled || IsGameOver) return;
        if (!_currentPlanet.IsPinned) return;

        UpdateCurrentPlanetPosition(mouseState);
        _currentPlanet.IsPinned = false;

        // Disable input
        _isInputEnabled = false;
    }

    private void HandleElapsedTime(GameTime gameTime)
    {
        _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        // If the elapsed time is greater than the delay, switch the planet and re-enable input
        if (!(_elapsedTime >= ClickDelay) || IsGameOver) return;
        SwitchPlanet();
        _isInputEnabled = true;
        _elapsedTime = 0;
    }

    private void SwitchPlanet()
    {
        // Switch the current planet with the next planet
        _currentPlanet = _nextPlanet;

        // Pin the planet
        _currentPlanet.IsPinned = true;

        // Add the planet to the scene
        _scene.AddPlanet(_currentPlanet);

        // Generate the next planet
        GenerateNextPlanet();
    }

    private void UpdateCurrentPlanetPosition(MouseState mouseState)
    {
        var x = mouseState.X;

        // Check if the planet is out of bounds
        // If it is, set the position to the closest edge of the container
        // Otherwise, set the position to the mouse's X coordinate
        if (x < _container.TopLeft.X + _currentPlanet.Radius)
        {
            _currentPlanet.Position.X = _container.TopLeft.X + _currentPlanet.Radius;
            _currentPlanet.OldPosition.X = _container.TopLeft.X + _currentPlanet.Radius;
        }
        else if (x > _container.BottomRight.X - _currentPlanet.Radius)
        {
            _currentPlanet.Position.X = _container.BottomRight.X - _currentPlanet.Radius;
            _currentPlanet.OldPosition.X = _container.BottomRight.X - _currentPlanet.Radius;
        }
        else
        {
            _currentPlanet.Position.X = x;
            _currentPlanet.OldPosition.X = x;
        }
    }

    private void UpdateScoreIfChanged()
    {
        if (!_score.HasChanged) return;
        _userInterfaceManager.ScoreText = _score.CurrentScore.ToString();
        _score.HasChanged = false;
    }

    public override void Draw()
    {
        Game.SpriteBatch.Begin();

        // Draw the user interface elements
        _userInterfaceManager.DrawInterfaceElements();

        // Draw the scene (planets and container)
        _scene.Draw(Game.SpriteBatch, Game.GraphicsDevice.Viewport.Height);

        // Draw the next planet
        _userInterfaceManager.DrawNextPlanet(_nextPlanet);

        Game.SpriteBatch.End();
    }
}