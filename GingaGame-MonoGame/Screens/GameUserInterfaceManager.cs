using System.Linq;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame;

public class GameUserInterfaceManager
{
    private const float EvolutionCycleScaleFactor = 0.4f;
    private const float DesiredFontHeight = 35;
    private readonly Game1 _game;
    private readonly GameMode _gameMode;

    public GameUserInterfaceManager(Game1 game, GameMode gameMode)
    {
        _game = game;
        _gameMode = gameMode;
    }

    private float NextPlanetFontScale { get; set; }
    private float ScoreFontScale { get; set; }
    private float TopScoresFontScale { get; set; }
    private Texture2D BackgroundTexture { get; set; }
    private Texture2D NextPlanetFontTexture { get; set; }
    private Texture2D ScoreFontTexture { get; set; }
    private Texture2D EvolutionCycleTexture { get; set; }
    private Texture2D TopScoresFontTexture { get; set; }
    private Texture2D NextPlanetTexture { get; set; }
    private SpriteFont Font { get; set; }
    public string ScoreText { get; set; }
    private string TopScoresText { get; set; }

    public void LoadContent()
    {
        LoadTexturesAndFont();
        CalculateScales();
    }

    private void LoadTexturesAndFont()
    {
        BackgroundTexture = LoadTexture("Resources/Background2");
        NextPlanetFontTexture = LoadTexture("Resources/NextPlanetFont");
        ScoreFontTexture = LoadTexture("Resources/ScoreFont");
        EvolutionCycleTexture = LoadTexture("Resources/EvolutionCycle");
        TopScoresFontTexture = LoadTexture("Resources/TopScoresFont");
        Font = _game.Content.Load<SpriteFont>("MyFont");
    }

    private Texture2D LoadTexture(string path)
    {
        return _game.Content.Load<Texture2D>(path);
    }

    private static float CalculateScale(float height)
    {
        return DesiredFontHeight / height;
    }

    private void CalculateScales()
    {
        NextPlanetFontScale = CalculateScale(NextPlanetFontTexture.Height);
        ScoreFontScale = CalculateScale(ScoreFontTexture.Height);
        TopScoresFontScale = CalculateScale(TopScoresFontTexture.Height - 18);
    }

    public void Initialize(Scoreboard scoreboard)
    {
        ScoreText = "0";

        UpdateScoreboardText(scoreboard);
    }

    public void UpdateScoreboardText(Scoreboard scoreboard)
    {
        TopScoresText = string.Join("\n",
            scoreboard.GetTopScores().Select(entry => $"{entry.PlayerName}: {entry.Score}"));
    }

    public void DrawInterfaceElements()
    {
        DrawBackground();
        DrawNextPlanetText();
        DrawScore();
        DrawTopScores();
        DrawEvolutionCycle();
    }

    private void DrawBackground()
    {
        _game.SpriteBatch.Draw(BackgroundTexture,
            new Rectangle(0, 0, _game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height),
            Color.White);
    }

    private void DrawNextPlanetText()
    {
        _game.SpriteBatch.Draw(NextPlanetFontTexture, new Vector2(65, 25), null, Color.White, 0, Vector2.Zero,
            NextPlanetFontScale, SpriteEffects.None, 0);
    }

    private void DrawTextSprite(Texture2D texture, Vector2 position, float scale)
    {
        _game.SpriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero,
            scale, SpriteEffects.None, 0);
    }

    private void DrawText(string text, Vector2 position)
    {
        _game.SpriteBatch.DrawString(Font, text, position, Color.White);
    }

    private void DrawScore()
    {
        DrawTextSprite(ScoreFontTexture, new Vector2(65, 281), ScoreFontScale);
        DrawText(ScoreText, new Vector2(190, 285));
    }

    private void DrawTopScores()
    {
        DrawTextSprite(TopScoresFontTexture, new Vector2(65, 365), TopScoresFontScale);
        DrawText(TopScoresText, new Vector2(65, 410));
    }

    private void DrawEvolutionCycle()
    {
        _game.SpriteBatch.Draw(EvolutionCycleTexture, new Vector2(20, 610), null, Color.White, 0, Vector2.Zero,
            EvolutionCycleScaleFactor, SpriteEffects.None, 0);
    }

    public void DrawNextPlanet(Planet nextPlanet)
    {
        NextPlanetTexture = PlanetTextures.GetCachedTexture(nextPlanet.PlanetType);
        var imageDimensions = new Vector2(nextPlanet.Radius * 2);

        // Calculate the position for the next planet
        var nextPlanetPosition = CalculateNextPlanetPosition(imageDimensions.X);

        _game.SpriteBatch.Draw(NextPlanetTexture,
            new Rectangle((int)nextPlanetPosition.X, (int)(nextPlanetPosition.Y - imageDimensions.Y / 2),
                (int)imageDimensions.X, (int)imageDimensions.Y), Color.White);
    }

    private Vector2 CalculateNextPlanetPosition(float imageWidth)
    {
        // Calculate the middle X position of the "Next Planet" text
        var nextPlanetTextMiddleX = 65 + NextPlanetFontTexture.Width * NextPlanetFontScale / 2;

        // Calculate the Y position of the "Score" text
        const int scoreTextY = 281;

        // Calculate the Y position of the "Next Planet" text
        var nextPlanetTextY = 25 + NextPlanetFontTexture.Height * NextPlanetFontScale;

        // Calculate the middle Y position
        var middleY = (scoreTextY + nextPlanetTextY) / 2;

        // The X position is the middle X position of the "Next Planet" text minus half of the planet's width
        // The Y position is the middle Y position
        return new Vector2(nextPlanetTextMiddleX - imageWidth / 2, middleY);
    }
}