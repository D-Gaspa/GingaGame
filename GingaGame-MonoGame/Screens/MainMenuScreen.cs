using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GingaGame_MonoGame;

public class MainMenuScreen : GameScreen
{
    private const float LogoScaleFactor = 0.35f;
    private Texture2D _backgroundTexture;
    private Rectangle _gameMode1ButtonRect;
    private Texture2D _gameMode1ButtonTexture;
    private Rectangle _gameMode2ButtonRect;
    private Texture2D _gameMode2ButtonTexture;
    private Rectangle _logoRect;
    private Texture2D _logoTexture;

    public MainMenuScreen(Game1 game) : base(game)
    {
    }

    private int CenterX => Game.GraphicsDevice.Viewport.Width / 2;
    private int CenterY => Game.GraphicsDevice.Viewport.Height / 2;

    public override void LoadContent()
    {
        LoadTextures();
        SetupLayout();
    }

    private void LoadTextures()
    {
        _backgroundTexture = LoadTexture("Resources/Background");
        _logoTexture = LoadTexture("Resources/Logo");
        _gameMode1ButtonTexture = LoadTexture("Resources/GameMode1Button");
        _gameMode2ButtonTexture = LoadTexture("Resources/GameMode2Button");
    }

    private Texture2D LoadTexture(string path)
    {
        return Game.Content.Load<Texture2D>(path);
    }

    private void SetupLayout()
    {
        // Set the position and size of the logo
        _logoRect = new Rectangle(CenterX - (int)(_logoTexture.Width * LogoScaleFactor) / 2, 50,
            (int)(_logoTexture.Width * LogoScaleFactor), (int)(_logoTexture.Height * LogoScaleFactor));

        // Set the position and size of the buttons
        _gameMode1ButtonRect = new Rectangle(CenterX - 175, CenterY - 50, 350, 100);
        _gameMode2ButtonRect = new Rectangle(CenterX - 175, CenterY + 100, 350, 100);
    }

    public override void Update(GameTime gameTime)
    {
        // Check if the game mode buttons are clicked
        if (Game1.MouseState.LeftButton != ButtonState.Pressed) return;
        if (_gameMode1ButtonRect.Contains(Game1.MouseState.Position))
            // Game mode 1 button was clicked
            Game.SwitchScreen(new GameMode1Screen(Game)); // Switch to game mode 1 screen
        else if (_gameMode2ButtonRect.Contains(Game1.MouseState.Position))
            // Game mode 2 button was clicked
            Game.SwitchScreen(new GameMode2Screen(Game)); // Switch to game mode 2 screen
    }

    public override void Draw(GameTime gameTime)
    {
        Game.SpriteBatch.Begin();

        DrawBackground();
        DrawLogo();
        DrawButton(_gameMode1ButtonTexture, _gameMode1ButtonRect);
        DrawButton(_gameMode2ButtonTexture, _gameMode2ButtonRect);

        Game.SpriteBatch.End();
    }

    private void DrawBackground()
    {
        Game.SpriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
    }

    private void DrawLogo()
    {
        Game.SpriteBatch.Draw(_logoTexture, new Vector2(_logoRect.X, _logoRect.Y), null, Color.White, 0f, Vector2.Zero,
            LogoScaleFactor, SpriteEffects.None, 0f);
    }

    private void DrawButton(Texture2D texture, Rectangle rectangle)
    {
        Game.SpriteBatch.Draw(texture, rectangle,
            rectangle.Contains(Game1.MouseState.Position) ? Color.LightGray : Color.White);
    }
}