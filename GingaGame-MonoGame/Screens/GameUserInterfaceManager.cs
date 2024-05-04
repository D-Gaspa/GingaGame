using System;
using System.Linq;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame;

/// <summary>
///     Responsible for managing the game user interface.
/// </summary>
public class GameUserInterfaceManager
{
    private const float EvolutionCycleScaleFactor = 0.4f;
    private const float DesiredFontHeight = 35;
    private readonly Game1 _game;
    private readonly GameMode _gameMode;
    private int _backgroundRepetitions;
    private Texture2D _backgroundTexture;
    private int _backgroundYOffset;
    private Texture2D _evolutionCycleTexture;
    private SpriteFont _font;
    private float _nextPlanetFontScale;
    private Texture2D _nextPlanetFontTexture;
    private Texture2D _nextPlanetTexture;
    private float _scoreFontScale;
    private Texture2D _scoreFontTexture;
    private float _topScoresFontScale;
    private Texture2D _topScoresFontTexture;
    private string _topScoresText;
    public string ScoreText;

    /// <summary>
    ///     Initializes a new instance of the <see cref="GameUserInterfaceManager" /> class.
    /// </summary>
    /// <param name="game">Reference to main game class.</param>
    /// <param name="gameMode">Current game mode.</param>
    public GameUserInterfaceManager(Game1 game, GameMode gameMode)
    {
        _game = game;
        _gameMode = gameMode;
    }

    /// <summary>
    ///     Loads content used by this class (textures, fonts, etc.).
    /// </summary>
    public void LoadContent()
    {
        LoadTexturesAndFont();
        CalculateScales();
    }

    /// <summary>
    ///     Loads required textures and font for the game user interface.
    /// </summary>
    private void LoadTexturesAndFont()
    {
        _backgroundTexture =
            LoadTexture(_gameMode == GameMode.Mode1 ? "Resources/Background2" : "Resources/ScrollerBackground1");

        _evolutionCycleTexture =
            LoadTexture(_gameMode == GameMode.Mode1 ? "Resources/EvolutionCycle" : "Resources/EvolutionCycle2");

        _nextPlanetFontTexture = LoadTexture("Resources/NextPlanetFont");
        _scoreFontTexture = LoadTexture("Resources/ScoreFont");
        _topScoresFontTexture = LoadTexture("Resources/TopScoresFont");
        _font = _game.Content.Load<SpriteFont>("MyFont");
    }

    /// <summary>
    ///     Loads a texture from the given path.
    /// </summary>
    /// <param name="path">The path to the texture.</param>
    /// <returns>The loaded Texture2D object.</returns>
    private Texture2D LoadTexture(string path)
    {
        return _game.Content.Load<Texture2D>(path);
    }

    /// <summary>
    ///     Calculates the scale based on the desired font height.
    /// </summary>
    /// <param name="height">The height to calculate the scale for.</param>
    /// <returns>The calculated scale.</returns>
    private static float CalculateScale(float height)
    {
        return DesiredFontHeight / height;
    }

    /// <summary>
    ///     Calculates the scales for the user interface elements based on the textures' heights.
    /// </summary>
    private void CalculateScales()
    {
        _nextPlanetFontScale = CalculateScale(_nextPlanetFontTexture.Height);
        _scoreFontScale = CalculateScale(_scoreFontTexture.Height);
        _topScoresFontScale = CalculateScale(_topScoresFontTexture.Height - 18);
    }

    /// <summary>
    ///     Initializes the GameUserInterfaceManager instance.
    /// </summary>
    /// <param name="scoreboard">The scoreboard object containing score data.</param>
    public void Initialize(Scoreboard scoreboard)
    {
        ScoreText = "0";

        UpdateScoreboardText(scoreboard);
    }

    /// <summary>
    ///     Updates the text displayed on the scoreboard.
    /// </summary>
    /// <param name="scoreboard">The scoreboard object containing score data.</param>
    public void UpdateScoreboardText(Scoreboard scoreboard)
    {
        _topScoresText = string.Join("\n",
            scoreboard.GetTopScores().Select(entry => $"{entry.PlayerName}: {entry.Score}"));
    }

    /// <summary>
    ///     Draws the interface elements on the screen.
    /// </summary>
    public void DrawInterfaceElements()
    {
        DrawBackground();
        DrawNextPlanetText();
        DrawScore();
        DrawTopScores();
        DrawEvolutionCycle();
    }

    /// <summary>
    ///     Prepares the background with an offset for parallax scrolling effect.
    /// </summary>
    /// <param name="backgroundYOffset">The vertical offset of the background.</param>
    /// <param name="screenHeight">The height of the screen.</param>
    /// <param name="verticalMargin">The vertical margin.</param>
    public void PrepareBackgroundWithOffset(int backgroundYOffset, int screenHeight, int verticalMargin)
    {
        _backgroundYOffset = backgroundYOffset;
        // Calculate how many background image repetitions are needed to cover the viewable area
        _backgroundRepetitions = (int)Math.Ceiling((screenHeight + 2 * verticalMargin + backgroundYOffset) /
                                                   (float)_backgroundTexture.Height);
    }

    /// <summary>
    ///     Draws the background based on the current game mode. The background is drawn differently for each game mode.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the game mode is not valid.</exception>
    private void DrawBackground()
    {
        switch (_gameMode)
        {
            case GameMode.Mode1:
                _game.SpriteBatch.Draw(_backgroundTexture,
                    new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height),
                    Color.White);
                break;
            case GameMode.Mode2:
            {
                // Draw the background image multiple times, offsetting it vertically for each repetition
                for (var i = 0; i < _backgroundRepetitions; i++)
                {
                    var yPosition = -_backgroundYOffset + i * _backgroundTexture.Height;
                    _game.SpriteBatch.Draw(_backgroundTexture,
                        new Rectangle(0, yPosition, _game.GraphicsDevice.Viewport.Width, _backgroundTexture.Height),
                        Color.White);
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    ///     Draws the next planet text on the game user interface.
    /// </summary>
    private void DrawNextPlanetText()
    {
        _game.SpriteBatch.Draw(_nextPlanetFontTexture, new Vector2(65, 25), null, Color.White, 0, Vector2.Zero,
            _nextPlanetFontScale, SpriteEffects.None, 0);
    }

    /// <summary>
    ///     Draws a sprite containing text on the screen.
    /// </summary>
    /// <param name="texture">The texture to be drawn on the screen.</param>
    /// <param name="position">The position at which the texture will be drawn.</param>
    /// <param name="scale">The scale factor applied to the texture.</param>
    private void DrawTextSprite(Texture2D texture, Vector2 position, float scale)
    {
        _game.SpriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero,
            scale, SpriteEffects.None, 0);
    }

    /// <summary>
    ///     Draws text at the specified position.
    /// </summary>
    /// <param name="text">The text to be drawn.</param>
    /// <param name="position">The position where the text should be drawn.</param>
    private void DrawText(string text, Vector2 position)
    {
        _game.SpriteBatch.DrawString(_font, text, position, Color.White);
    }

    /// <summary>
    ///     Draws the score on the game user interface.
    /// </summary>
    private void DrawScore()
    {
        DrawTextSprite(_scoreFontTexture, new Vector2(65, 281), _scoreFontScale);
        DrawText(ScoreText, new Vector2(190, 285));
    }

    /// <summary>
    ///     Draws the top scores on the game interface.
    /// </summary>
    private void DrawTopScores()
    {
        DrawTextSprite(_topScoresFontTexture, new Vector2(65, 365), _topScoresFontScale);
        DrawText(_topScoresText, new Vector2(65, 410));
    }

    /// <summary>
    ///     Represents a class responsible for managing the game user interface.
    /// </summary>
    private void DrawEvolutionCycle()
    {
        _game.SpriteBatch.Draw(_evolutionCycleTexture, new Vector2(20, 610), null, Color.White, 0, Vector2.Zero,
            EvolutionCycleScaleFactor, SpriteEffects.None, 0);
    }

    /// <summary>
    ///     Draws the next planet on the game screen.
    /// </summary>
    /// <param name="nextPlanet">The planet object representing the next planet to draw.</param>
    public void DrawNextPlanet(Planet nextPlanet)
    {
        _nextPlanetTexture = PlanetTextures.GetCachedTexture(nextPlanet.PlanetType);
        var imageDimensions = new Vector2(nextPlanet.Radius * 2);

        // Calculate the position for the next planet
        var nextPlanetPosition = CalculateNextPlanetPosition(imageDimensions.X);

        _game.SpriteBatch.Draw(_nextPlanetTexture,
            new Rectangle((int)nextPlanetPosition.X, (int)(nextPlanetPosition.Y - imageDimensions.Y / 2),
                (int)imageDimensions.X, (int)imageDimensions.Y), Color.White);
    }

    /// <summary>
    ///     Calculates the position for the next planet.
    /// </summary>
    /// <param name="imageWidth">The width of the planet image.</param>
    /// <returns>The position for the next planet.</returns>
    private Vector2 CalculateNextPlanetPosition(float imageWidth)
    {
        // Calculate the middle X position of the "Next Planet" text
        var nextPlanetTextMiddleX = 65 + _nextPlanetFontTexture.Width * _nextPlanetFontScale / 2;

        // Calculate the Y position of the "Score" text
        const int scoreTextY = 281;

        // Calculate the Y position of the "Next Planet" text
        var nextPlanetTextY = 25 + _nextPlanetFontTexture.Height * _nextPlanetFontScale;

        // Calculate the middle Y position
        var middleY = (scoreTextY + nextPlanetTextY) / 2;

        // The X position is the middle X position of the "Next Planet" text minus half of the planet's width
        // The Y position is the middle Y position
        return new Vector2(nextPlanetTextMiddleX - imageWidth / 2, middleY);
    }
}