using System.Linq;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;
using Container = GingaGame_MonoGame.GameLogic.Container;

namespace GingaGame_MonoGame;

public class GameMode1Screen : GameScreen
{
    private const GameMode Mode = GameMode.Mode1;
    private const float DesiredFontHeight = 35;
    private const float EvolutionCycleScaleFactor = 0.4f;
    private const double ClickDelay = 0.5; // Delay in seconds
    private readonly CollisionManager _collisionManager;
    private readonly Container _container;
    private readonly GameStateHandler _gameStateHandler;
    private readonly PlanetFactory _planetFactory;
    private readonly Scene _scene;
    private readonly Score _score;
    private readonly Scoreboard _scoreboard;
    private Texture2D _backgroundTexture;
    private Planet _currentPlanet;
    private double _elapsedTime;
    private Texture2D _evolutionCycleTexture;
    private SpriteFont _font;
    private bool _isInputEnabled = true;
    private Planet _nextPlanet;
    private float _nextPlanetFontScale;
    private Texture2D _nextPlanetFontTexture;
    private Texture2D _nextPlanetTexture;
    private float _scoreFontScale;
    private Texture2D _scoreFontTexture;
    private string _scoreText;
    private float _topScoresFontScale;
    private Texture2D _topScoresFontTexture;
    private string _topScoresText;

    public GameMode1Screen(Game1 game, Desktop desktop) : base(game)
    {
        // Set the content manager for the PlanetTextures class
        PlanetTextures.SetContentManager(Game.Content);

        PlanetTextures.InitializePlanetTextures();

        _scene = new Scene();

        _container = new Container();

        _planetFactory = new PlanetFactory(Mode);

        _score = new Score();
        _scoreboard = new Scoreboard(Mode);

        var userInterfaceCreator = new UserInterfaceCreator(desktop);
        _gameStateHandler = new GameStateHandler(_container, this, _score, _scoreboard, userInterfaceCreator);

        var planetMergingService = new PlanetMergingService(_scene, Mode, _planetFactory, _score);
        var constraintHandler = new ConstraintHandler(_container, Mode, _scene);
        _collisionManager = new CollisionManager(constraintHandler, Mode, _gameStateHandler,
            planetMergingService, _scene);

        var middleX = Game.GraphicsDevice.Viewport.Width / 2;

        _currentPlanet = new Planet(PlanetType.Pluto, new Vector2(middleX, 0))
        {
            IsPinned = true
        };

        _scene.AddPlanet(_currentPlanet);

        GenerateNextPlanet();
    }

    public override void LoadContent()
    {
        LoadTexturesAndFont();
        CalculateScales();
        InitializeElements();
    }

    private void LoadTexturesAndFont()
    {
        _backgroundTexture = LoadTexture("Resources/Background2");
        _nextPlanetFontTexture = LoadTexture("Resources/NextPlanetFont");
        _scoreFontTexture = LoadTexture("Resources/ScoreFont");
        _evolutionCycleTexture = LoadTexture("Resources/EvolutionCycle");
        _topScoresFontTexture = LoadTexture("Resources/TopScoresFont");
        _font = Game.Content.Load<SpriteFont>("MyFont");
    }

    private Texture2D LoadTexture(string path)
    {
        return Game.Content.Load<Texture2D>(path);
    }

    private static float CalculateScale(float height)
    {
        return DesiredFontHeight / height;
    }

    private void CalculateScales()
    {
        _nextPlanetFontScale = CalculateScale(_nextPlanetFontTexture.Height);
        _scoreFontScale = CalculateScale(_scoreFontTexture.Height);
        _topScoresFontScale = CalculateScale(_topScoresFontTexture.Height - 18);
    }

    private void InitializeElements()
    {
        _scoreText = "0";

        UpdateScoreboardText();

        _container.InitializeContainer(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width,
            Game.GraphicsDevice.Viewport.Height);
    }

    private void UpdateScoreboardText()
    {
        _topScoresText = string.Join("\n",
            _scoreboard.GetTopScores().Select(entry => $"{entry.PlayerName}: {entry.Score}"));
    }

    public override void ResetGame()
    {
        // Reset the game state and initialize the game again
        _scene.ClearPlanets();
        _score.ResetScore();
        UpdateScoreboardText();
        _planetFactory.InitializeDefaultPlanetByGameMode();
        
        _elapsedTime = 0;
        _isInputEnabled = true;

        _currentPlanet.Position = new Vector2(0, 0);
        _currentPlanet.IsPinned = true;

        _scene.AddPlanet(_currentPlanet);

        GenerateNextPlanet();
    }

    private void GenerateNextPlanet()
    {
        _nextPlanet = _planetFactory.GenerateNextPlanet(Game.GraphicsDevice.Viewport.Width);
    }

    public override void Update(GameTime gameTime)
    {
        // Get the current mouse state
        var mouseState = Mouse.GetState();

        // Check if the left mouse button is pressed and input is enabled
        if (mouseState.LeftButton == ButtonState.Pressed && _isInputEnabled)
            // Check if the current planet is pinned
            if (_currentPlanet.IsPinned)
            {
                UpdateCurrentPlanetPosition(mouseState);
                _currentPlanet.IsPinned = false;

                // Disable input
                _isInputEnabled = false;
            }

        // If input is disabled, increment the elapsed time
        if (!_isInputEnabled)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            // If the elapsed time is greater than the delay, switch the planet and re-enable input
            if (_elapsedTime >= ClickDelay)
            {
                SwitchPlanet();
                _isInputEnabled = true;
                _elapsedTime = 0;
            }
        }

        // Check if the current planet is pinned
        if (_currentPlanet.IsPinned) UpdateCurrentPlanetPosition(mouseState);

        // Update the planet positions
        _scene.Update();

        // Check for collisions
        _collisionManager.RunCollisions(5);

        if (_score.HasChanged)
        {
            _scoreText = _score.CurrentScore.ToString();
            _score.HasChanged = false;
        }

        _gameStateHandler.Update();
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

    public override void Draw()
    {
        Game.SpriteBatch.Begin();

        DrawInterfaceElements();
        _scene.Draw(Game.SpriteBatch, Game.GraphicsDevice.Viewport.Height);
        _container.Draw(Game.SpriteBatch);
        DrawNextPlanet();

        Game.SpriteBatch.End();
    }

    private void DrawInterfaceElements()
    {
        DrawBackground();
        DrawNextPlanetText();
        DrawScore();
        DrawTopScores();
        DrawEvolutionCycle();
    }

    private void DrawBackground()
    {
        Game.SpriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
    }

    private void DrawNextPlanetText()
    {
        Game.SpriteBatch.Draw(_nextPlanetFontTexture, new Vector2(65, 25), null, Color.White, 0, Vector2.Zero,
            _nextPlanetFontScale, SpriteEffects.None, 0);
    }

    private void DrawScore()
    {
        // Draw the score text
        Game.SpriteBatch.Draw(_scoreFontTexture, new Vector2(65, 281), null, Color.White, 0, Vector2.Zero,
            _scoreFontScale, SpriteEffects.None, 0);
        // Draw the actual score value
        Game.SpriteBatch.DrawString(_font, _scoreText, new Vector2(190, 285), Color.White);
    }

    private void DrawTopScores()
    {
        // Draw the top scores text
        Game.SpriteBatch.Draw(_topScoresFontTexture, new Vector2(65, 365), null, Color.White, 0, Vector2.Zero,
            _topScoresFontScale, SpriteEffects.None, 0);
        // Draw the actual top scores
        Game.SpriteBatch.DrawString(_font, _topScoresText, new Vector2(65, 410), Color.White);
    }

    private void DrawEvolutionCycle()
    {
        Game.SpriteBatch.Draw(_evolutionCycleTexture, new Vector2(20, 610), null, Color.White, 0, Vector2.Zero,
            EvolutionCycleScaleFactor, SpriteEffects.None, 0);
    }

    private void DrawNextPlanet()
    {
        _nextPlanetTexture = PlanetTextures.GetCachedTexture(_nextPlanet.PlanetType);

        var imageWidth = _nextPlanet.Radius * 2;
        var imageHeight = _nextPlanet.Radius * 2;

        // Calculate the middle X position of the "Next Planet" text
        var nextPlanetTextMiddleX = 65 + _nextPlanetFontTexture.Width * _nextPlanetFontScale / 2;

        // Calculate the Y position of the "Score" text
        const int scoreTextY = 281;

        // Calculate the Y position of the "Next Planet" text
        var nextPlanetTextY = 25 + _nextPlanetFontTexture.Height * _nextPlanetFontScale;

        // Calculate the middle Y position
        var middleY = (scoreTextY + nextPlanetTextY) / 2;

        // Calculate the position for the next planet
        // The X position is the middle X position of the "Next Planet" text minus half of the planet's width
        // The Y position is the middle Y position
        var nextPlanetPosition = new Vector2(nextPlanetTextMiddleX - imageWidth / 2, middleY);

        Game.SpriteBatch.Draw(_nextPlanetTexture,
            new Rectangle((int)nextPlanetPosition.X, (int)(nextPlanetPosition.Y - imageHeight / 2),
                (int)imageWidth, (int)imageHeight), Color.White);
    }
}