using System.Linq;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Enforces constraints for the movement of a Planet, restricting it within various game boundaries.
/// </summary>
public class ConstraintHandler
{
    private readonly Container _container;
    private readonly GameMode _gameMode;
    private readonly Scene _scene;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ConstraintHandler"/> class.
    /// </summary>
    /// <param name="gameMode">The current game mode.</param>
    /// <param name="scene">The current game scene.</param>
    public ConstraintHandler(GameMode gameMode, Scene scene)
    {
        _container = scene.Container;
        _gameMode = gameMode;
        _scene = scene;
    }

    /// <summary>
    ///     Enforces screen boundaries for a planet's position.
    /// </summary>
    /// <param name="planet">The planet to enforce the constraints on.</param>
    public static void ScreenConstraints(Planet planet)
    {
        // Check if the point is outside the top boundary of the screen
        if (planet.Position.Y < planet.Radius) planet.Position.Y = planet.Radius;
    }

    /// <summary>
    ///     Enforces container boundaries for a planet's position.
    /// </summary>
    /// <param name="planet">The planet to enforce the constraints on.</param>
    public void ContainerConstraints(Planet planet)
    {
        // Check if the point is outside the left boundary of the container
        if (_container != null && planet.Position.X < _container.TopLeft.X + planet.Radius)
            planet.Position.X = _container.TopLeft.X + planet.Radius;

        // Check if the point is outside the right boundary of the container
        if (_container != null && planet.Position.X > _container.TopRight.X - planet.Radius)
            planet.Position.X = _container.TopRight.X - planet.Radius;

        // Check if the point is outside the bottom boundary of the container
        if (_container != null && planet.Position.Y > _container.BottomLeft.Y - planet.Radius)
            planet.Position.Y = _container.BottomLeft.Y - planet.Radius;
    }

    /// <summary>
    ///     Enforces floor boundaries for a planet's position according to game mode.
    /// </summary>
    /// <param name="planet">The planet to enforce the constraints on.</param>
    public void FloorConstraints(Planet planet)
    {
        if (_gameMode != GameMode.Mode2) return; // Apply only in GameMode2

        // Find the current floor
        var floor = _scene.Floors.FirstOrDefault(f =>
            f.StartPositionY <= planet.Position.Y && planet.Position.Y <= f.EndPositionY);

        if (floor == null) return; // Planet is outside the floor range

        // Check if the planet can pass through the floor
        if ((int)planet.PlanetType <= floor.NextPlanetIndex)
            // Can pass - no collision
            return;

        const int floorEndPositionHeight = 50; // The height of the rectangle at the end of the floor

        // Handle Collision (similar to container boundaries)
        if (!(planet.Position.Y > floor.EndPositionY - floorEndPositionHeight - planet.Radius)) return;

        planet.Position.Y = floor.EndPositionY - floorEndPositionHeight - planet.Radius;
    }
}