using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame;

/// <summary>
///     Represents the main menu screen of the game.
///     Contains logic for displaying the screen and processing user interactions.
/// </summary>
public class MainMenuScreen : GameScreen
{
    private const float LogoScaleFactor = 0.35f;
    private readonly Desktop _desktop;
    private readonly ComboView _levelSelector;
    private Texture2D _backgroundTexture;
    private Rectangle _gameMode1ButtonRect;
    private Texture2D _gameMode1ButtonTexture;
    private Rectangle _gameMode2ButtonRect;
    private Texture2D _gameMode2ButtonTexture;
    private Rectangle _logoRect;
    private Texture2D _logoTexture;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainMenuScreen" /> class.
    /// </summary>
    /// <param name="game"> A reference to the main Game instance. </param>
    /// <param name="desktop">The desktop UI manager.</param>
    public MainMenuScreen(Game1 game, Desktop desktop) : base(game)
    {
        _desktop = desktop;

        _levelSelector = new ComboView();

        _levelSelector.Widgets.Add(new Label { Text = "Level 1" });
        _levelSelector.Widgets.Add(new Label { Text = "Level 2" });
        _levelSelector.Widgets.Add(new Label { Text = "Level 3" });

        _levelSelector.SelectedIndex = 0; // Select the first item by default

        _desktop.Widgets.Add(_levelSelector);
    }

    /// <summary>
    ///     Gets the X-coordinate of the center of the screen.
    /// </summary>
    private int CenterX => Game.GraphicsDevice.Viewport.Width / 2;

    /// <summary>
    ///     Gets the Y-coordinate of the center of the screen.
    /// </summary>
    private int CenterY => Game.GraphicsDevice.Viewport.Height / 2;

    /// <summary>
    ///     Load the visual assets needed for this screen.
    /// </summary>
    public override void LoadContent()
    {
        LoadTextures();
        SetupLayout();
    }

    /// <summary>
    ///     Load the textures needed for the main menu screen.
    /// </summary>
    private void LoadTextures()
    {
        _backgroundTexture = LoadTexture("Resources/Background");
        _logoTexture = LoadTexture("Resources/Logo");
        _gameMode1ButtonTexture = LoadTexture("Resources/GameMode1Button");
        _gameMode2ButtonTexture = LoadTexture("Resources/GameMode2Button");
    }

    /// <summary>
    ///     Load a texture from the specified path.
    /// </summary>
    /// <param name="path">The path to the texture.</param>
    /// <returns></returns>
    private Texture2D LoadTexture(string path)
    {
        return Game.Content.Load<Texture2D>(path);
    }

    /// <summary>
    ///     Set up the layout of the main menu screen.
    /// </summary>
    private void SetupLayout()
    {
        // Set the position and size of the logo
        _logoRect = new Rectangle(CenterX - (int)(_logoTexture.Width * LogoScaleFactor) / 2, 50,
            (int)(_logoTexture.Width * LogoScaleFactor), (int)(_logoTexture.Height * LogoScaleFactor));

        // Set the position and size of the buttons
        _gameMode1ButtonRect = new Rectangle(CenterX - 175, CenterY - 50, 350, 100);
        _gameMode2ButtonRect = new Rectangle(CenterX - 175, CenterY + 100, 350, 100);
    }

    /// <summary>
    ///     Updates the state of the screen according to the provided game time.
    /// </summary>
    /// <param name="gameTime">The current game time snapshot.</param>
    public override void Update(GameTime gameTime)
    {
        var selectedLevel = ((Label)_levelSelector.SelectedItem).Text;

        // Check if the game mode buttons are clicked
        if (Game1.MouseState.LeftButton != ButtonState.Pressed) return;
        if (_gameMode1ButtonRect.Contains(Game1.MouseState.Position))
        {
            // Game mode 1 button was clicked
            Game.SwitchScreen(new GameMode1Screen(Game, _desktop)); // Switch to game mode 1 screen

            _levelSelector.Visible = false;
        }
        else if (_gameMode2ButtonRect.Contains(Game1.MouseState.Position))
        {
            // Game mode 2 button was clicked
            Game.SwitchScreen(new GameMode2Screen(Game, _desktop, selectedLevel)); // Switch to game mode 2 screen

            _levelSelector.Visible = false;
        }
    }

    /// <summary>
    ///     Draws the content of the main menu screen.
    /// </summary>
    public override void Draw()
    {
        Game.SpriteBatch.Begin();

        DrawBackground();
        DrawLogo();
        DrawButton(_gameMode1ButtonTexture, _gameMode1ButtonRect);
        DrawButton(_gameMode2ButtonTexture, _gameMode2ButtonRect);

        Game.SpriteBatch.End();
    }

    /// <summary>
    ///     Draws the background of the main menu screen.
    /// </summary>
    private void DrawBackground()
    {
        Game.SpriteBatch.Draw(_backgroundTexture,
            new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), Color.White);
    }

    /// <summary>
    ///     Draws the logo of the game.
    /// </summary>
    private void DrawLogo()
    {
        Game.SpriteBatch.Draw(_logoTexture, new Vector2(_logoRect.X, _logoRect.Y), null, Color.White, 0f, Vector2.Zero,
            LogoScaleFactor, SpriteEffects.None, 0f);
    }

    /// <summary>
    ///     Draws a button with the specified texture and rectangle.
    /// </summary>
    /// <param name="texture">The texture of the button.</param>
    /// <param name="rectangle">The rectangle defining the position and size of the button.</param>
    private void DrawButton(Texture2D texture, Rectangle rectangle)
    {
        Game.SpriteBatch.Draw(texture, rectangle,
            rectangle.Contains(Game1.MouseState.Position) ? Color.LightGray : Color.White);
    }

    /// <summary>
    ///     Resets the state of the game screen. No-op for the main menu screen.
    /// </summary>
    public override void ResetGame()
    {
        // No game state to reset in the main menu
    }

    /// <summary>
    ///     Resumes the game logic after being paused. No-op for the main menu screen.
    /// </summary>
    public override void ResumeGame()
    {
        // No input to enable in the main menu
    }

    /// <summary>
    ///     Pause the game logic, typically allowing navigation in menus, etc. No-op for the main menu screen.
    /// </summary>
    public override void PauseGame()
    {
        // No input to disable in the main menu
    }
}