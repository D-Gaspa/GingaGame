using System;
using System.Collections.Generic;
using System.Linq;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame;

public class GameMode2Screen : GameModeScreenBase
{
    private const float ParallaxBackgroundFactor = 0.1f;

    private readonly Dictionary<string, string> _mapData = new()
    {
        { "VerticalMargin", "120" },
        { "HorizontalMargin", "0" },
        { "NumberOfFloors", "4->4,3,3,1" }
    };

    private int _backgroundYOffset;
    private int _currentFloorIndex;
    private Planet _currentPlanet;
    private bool _followPlanet;
    private int _horizontalMargin;
    private int _numberOfFloors;
    private List<int> _planetsPerFloor = new();
    private int _scrollOffset;
    private int _verticalMargin = 120;


    public GameMode2Screen(Game1 game, Desktop desktop) : base(game, desktop)
    {
        // Game components are initialized in the base class
    }

    protected override GameMode Mode => GameMode.Mode2;
    private int FloorHeight => Game.GraphicsDevice.Viewport.Height - 2 * _verticalMargin;

    protected override void InitializeGameSpecificComponents()
    {
        // Game mode 2-specific code
        var planetMergingService = new PlanetMergingService(Scene, Mode, PlanetFactory, Score, this);
        var constraintHandler = new ConstraintHandler(Mode, Scene);
        CollisionManager = new CollisionManager(constraintHandler, Mode, GameStateHandler,
            planetMergingService, Scene);
    }

    protected override void InitializeElements()
    {
        base.InitializeElements();

        LoadGameMode2Map(_mapData);

        Scene.InitializeFloors(FloorHeight, _verticalMargin, _numberOfFloors, _planetsPerFloor);

        // Calculate container size
        var containerHeight = Scene.Floors.Count * FloorHeight + _verticalMargin;

        Container.InitializeContainer(Game.GraphicsDevice, containerHeight, Game.GraphicsDevice.Viewport.Width, Mode,
            _verticalMargin, _verticalMargin, _horizontalMargin);
    }

    private void LoadGameMode2Map(Dictionary<string, string> mapData)
    {
        foreach (var (key, value) in mapData)
            switch (key)
            {
                case "VerticalMargin":
                    _verticalMargin = int.Parse(value);
                    break;
                case "HorizontalMargin":
                    _horizontalMargin = int.Parse(value);
                    break;
                case "NumberOfFloors":
                    var floorData = value.Split("->");
                    _numberOfFloors = int.Parse(floorData[0]);
                    _planetsPerFloor = floorData[1].Split(',').Select(int.Parse).ToList();
                    break;
            }

        // Validation
        if (_numberOfFloors is < 2 or > 10)
            throw new Exception("Invalid number of floors. It should be between 2 and 10.");

        if (_planetsPerFloor.Count != _numberOfFloors)
            throw new Exception("The number of floors should match the number of planets per floor.");

        if (_planetsPerFloor.Sum() != 11)
            throw new Exception("The sum of the number of planets for each floor should be 11.");

        if (_planetsPerFloor.Last() != 1) throw new Exception("The last number should always be 1.");

        if (_planetsPerFloor.Any(p => p is 0))
            throw new Exception("The number of planets per floor should be greater than 0.");
    }

    public override void Update(GameTime gameTime)
    {
        if (IsInputEnabled)
        {
            var keyboardState = Keyboard.GetState();
            HandleKeyboardInput(keyboardState);
        }
        
        base.Update(gameTime);
    }
    
    private void HandleKeyboardInput(KeyboardState keyboardState)
    {
        const int scrollSpeed = 35;

        if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
        {
            if (_scrollOffset < scrollSpeed) return;
            _scrollOffset -= scrollSpeed;
            DeselectCurrentPlanet();
        }
        else if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
        {
            if (_scrollOffset >= (Scene.Floors.Count - 1) * FloorHeight - _verticalMargin + scrollSpeed) return;
            _scrollOffset += scrollSpeed;
            DeselectCurrentPlanet();
        }
        else if (keyboardState.IsKeyDown(Keys.Back))
        {
            // Reset scroll offset
            _scrollOffset = 0;
            DeselectCurrentPlanet();
        }
        else if (keyboardState.IsKeyDown(Keys.Enter))
        {
            if (!CurrentPlanetToDrop.IsPinned) return;
            // First go back to the top if not already there
            if (_scrollOffset > 0)
            {
                _scrollOffset = 0;
                DeselectCurrentPlanet();
            }
            else
            {
                // Drop the current planet
                CurrentPlanetToDrop.IsPinned = false;
                _currentPlanet = CurrentPlanetToDrop;

                // Disable input immediately
                IsInputEnabled = false;
            }
        }
    }

    protected override void HandleMouseClick(MouseState mouseState)
    {
        // Check if the left mouse button is pressed and input is enabled
        if (mouseState.LeftButton != ButtonState.Pressed || !IsInputEnabled || IsGameOver) return;
        if (!CurrentPlanetToDrop.IsPinned) return;

        // First go back to the top if not already there
        if (_scrollOffset > 0)
        {
            _scrollOffset = 0;
            DeselectCurrentPlanet();
        }
        else
        {
            UpdateCurrentPlanetPosition(mouseState);
            CurrentPlanetToDrop.IsPinned = false;
            _currentPlanet = CurrentPlanetToDrop;

            // Disable input
            IsInputEnabled = false;
        }
    }

    private void DeselectCurrentPlanet()
    {
        _currentPlanet = null;
    }

    public override void Draw()
    {
        // Scrolling Logic
        CalculateScrollOffset();

        // Parallax Background Rendering
        _backgroundYOffset = (int)(_scrollOffset * ParallaxBackgroundFactor);

        UserInterfaceManager.PrepareBackgroundWithOffset(_backgroundYOffset, Game.GraphicsDevice.Viewport.Height,
            _verticalMargin);

        base.Draw();
    }

    private void CalculateScrollOffset()
    {
        if (_currentPlanet == null) return;

        // Find the current floor
        var currentFloor = Scene.Floors.FirstOrDefault(f =>
            f.StartPositionY <= _currentPlanet.Position.Y && _currentPlanet.Position.Y <= f.EndPositionY);

        if (currentFloor == null) return; // Planet is outside the floor range

        // Update the current floor index
        _currentFloorIndex = currentFloor.Index;

        // Calculate the scroll offset based on the current floor
        _scrollOffset = FloorHeight * _currentFloorIndex;

        // Gradually adjust the offset for a smooth transition when following the planet
        if (_followPlanet) _scrollOffset += (int)(_currentPlanet.Position.Y - currentFloor.StartPositionY);
    }

    protected override void DrawScene()
    {
        Scene.Draw(Game.SpriteBatch, Game.GraphicsDevice.Viewport.Height, _scrollOffset);
    }

    public Planet GetCurrentPlanet()
    {
        return _currentPlanet;
    }

    public void SetCurrentPlanet(Planet planet)
    {
        _currentPlanet = planet;
    }
}