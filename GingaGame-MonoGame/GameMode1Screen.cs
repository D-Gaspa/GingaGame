using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame;

public class GameMode1Screen : GameScreen
{
    private const float DesiredFontHeight = 35;
    private const float EvolutionCycleScaleFactor = 0.4f;
    private Texture2D _backgroundTexture;
    private Texture2D _evolutionCycleTexture;
    private SpriteFont _font;
    private float _nextPlanetFontScale;
    private Texture2D _nextPlanetFontTexture;
    private Texture2D _nextPlanetTexture;
    private float _scoreFontScale;
    private Texture2D _scoreFontTexture;
    private string _scoreText;
    private float _topScoresFontScale;
    private Texture2D _topScoresFontTexture;
    private string _topScoresText;

    public GameMode1Screen(Game1 game) : base(game)
    {
    }

    public override void LoadContent()
    {
        // Load the textures and fonts
        _backgroundTexture = Game.Content.Load<Texture2D>("Resources/Background2");
        _nextPlanetFontTexture = Game.Content.Load<Texture2D>("Resources/NextPlanetFont");
        _scoreFontTexture = Game.Content.Load<Texture2D>("Resources/ScoreFont");
        _evolutionCycleTexture = Game.Content.Load<Texture2D>("Resources/EvolutionCycle");
        _topScoresFontTexture = Game.Content.Load<Texture2D>("Resources/TopScoresFont");
        _font = Game.Content.Load<SpriteFont>("MyFont");

        // Calculate the scale factors
        _nextPlanetFontScale = DesiredFontHeight / _nextPlanetFontTexture.Height;
        _scoreFontScale = DesiredFontHeight / _scoreFontTexture.Height;
        _topScoresFontScale = DesiredFontHeight / (_topScoresFontTexture.Height - 18);

        // Initialize the text values
        _scoreText = "0";
        _topScoresText = "1. ...\n2. ...\n3. ...\n4. ...\n5. ..."; // TODO: Replace with actual scores
    }

    public override void Update(GameTime gameTime)
    {
        // Update your screen here
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Begin();

        DrawInterfaceElements();

        Game.SpriteBatch.End();
    }

    private void DrawInterfaceElements()
    {
        DrawBackground();
        DrawNextPlanet();
        DrawScore();
        DrawTopScores();
        DrawEvolutionCycle();
    }

    private void DrawBackground()
    {
        Game.SpriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
    }

    private void DrawNextPlanet()
    {
        Game.SpriteBatch.Draw(_nextPlanetFontTexture, new Vector2(65, 25), null, Color.White, 0, Vector2.Zero,
            _nextPlanetFontScale, SpriteEffects.None, 0);
        // TODO: Draw the next planet texture
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
}