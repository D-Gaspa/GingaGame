using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Myra;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame;

/// <summary>
///     Represents the main game class.
/// </summary>
public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly Stack<GameScreen> _screens = new();
    private Desktop _desktop;
    public SpriteBatch SpriteBatch;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Game1" /> class.
    /// </summary>
    public Game1()
    {
        // Set the graphics device manager
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    /// <summary>
    ///     Gets the current keyboard state.
    /// </summary>
    public static MouseState MouseState => Mouse.GetState();

    /// <summary>
    ///     Initialize the game: set graphics and load initial content.
    /// </summary>
    protected override void Initialize()
    {
        // Set the game to fullscreen
        _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
        //_graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        // Set the Game instance for Myra
        MyraEnvironment.Game = this;

        _desktop = new Desktop();

        // Push the main menu screen to the stack
        _screens.Push(new MainMenuScreen(this, _desktop));

        base.Initialize();
    }

    /// <summary>
    ///     Loads the game content. This method will be called once per game and is the place to load
    ///     all the content.
    /// </summary>
    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // Call the LoadContent method of the top screen
        _screens.Peek().LoadContent();
    }

    /// <summary>
    ///     Updates the state of the top screen in the stack.
    /// </summary>
    /// <param name="gameTime">The current game time snapshot.</param>
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Call the Update method of the top screen
        _screens.Peek().Update(gameTime);
        base.Update(gameTime);
    }

    /// <summary>
    ///     This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Call the Draw method of the top screen
        _screens.Peek().Draw();

        _desktop.Render();

        base.Draw(gameTime);
    }

    /// <summary>
    ///     Switches the current screen to the given screen.
    /// </summary>
    /// <param name="screen">The screen to switch to.</param>
    public void SwitchScreen(GameScreen screen)
    {
        _screens.Pop();
        _screens.Push(screen);
        screen.LoadContent();
    }
}