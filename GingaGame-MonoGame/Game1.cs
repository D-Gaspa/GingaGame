using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GingaGame_MonoGame;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private readonly Stack<GameScreen> _screens = new();
    public SpriteBatch SpriteBatch;

    public Game1()
    {
        // Set the graphics device manager
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    public static MouseState MouseState => Mouse.GetState();

    protected override void Initialize()
    {
        // Set the game to fullscreen
        _graphics.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
        _graphics.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
        // _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        // Push the main menu screen to the stack
        _screens.Push(new MainMenuScreen(this));
        base.Initialize();
    }

    protected override void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        // Call the LoadContent method of the top screen
        _screens.Peek().LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Call the Update method of the top screen
        _screens.Peek().Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Call the Draw method of the top screen
        _screens.Peek().Draw(gameTime);
        base.Draw(gameTime);
    }

    public void SwitchScreen(GameScreen screen)
    {
        _screens.Pop();
        _screens.Push(screen);
        screen.LoadContent();
    }
}