namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Manages collision detection and resolution within the game.
/// </summary>
public class CollisionManager
{
    private readonly ConstraintHandler _constraintHandler;
    private readonly CollisionDetector _detector;
    private readonly GameMode _gameMode;
    private readonly CollisionResolver _resolver;
    private readonly Scene _scene;

    /// <summary>
    ///     Constructor that initializes a new instance of the <see cref="CollisionManager"/> class and its dependencies.
    /// </summary>
    /// <param name="constraintHandler">An object used to manage constraints.</param>
    /// <param name="gameMode">The current game mode.</param>
    /// <param name="gameStateHandler">Denotes the state of the game.</param>
    /// <param name="planetMergingService">Service for merging planets.</param>
    /// <param name="scene">The current game scene.</param>
    public CollisionManager(ConstraintHandler constraintHandler, GameMode gameMode, GameStateHandler gameStateHandler,
        PlanetMergingService planetMergingService, Scene scene)
    {
        _detector = new CollisionDetector(scene.Planets);
        _resolver = new CollisionResolver(gameStateHandler, planetMergingService);

        _constraintHandler = constraintHandler;

        _gameMode = gameMode;
        _scene = scene;
    }

    /// <summary>
    ///     Runs collision detection and reaction logic according to the specified number of iterations.
    /// </summary>
    /// <remarks>
    ///     Too many iterations -> more stable, slower and less responsive.
    ///     Too few iterations -> less stable, faster and more responsive.
    ///     8 iterations is a good balance for this game.
    /// </remarks>
    /// <param name="iterations">The number of times the collision logic should repeat.</param>
    public void RunCollisions(int iterations)
    {
        for (var i = 0; i < iterations; i++)
        {
            CheckConstraints();
            RunCollisions();
        }
    }

    /// <summary>
    ///     Checks all the planet's constraints.
    /// </summary>
    private void CheckConstraints()
    {
        foreach (var planet in _scene.Planets)
        {
            ConstraintHandler.ScreenConstraints(planet);
            _constraintHandler.ContainerConstraints(planet);
            if (_gameMode == GameMode.Mode2) _constraintHandler.FloorConstraints(planet);
        }
    }

    /// <summary>
    ///     Runs collision detection and resolves all collisions found.
    /// </summary>
    private void RunCollisions()
    {
        var potentialCollisions = _detector.CheckCollisions();
        _resolver.HandleCollisions(potentialCollisions);
    }
}