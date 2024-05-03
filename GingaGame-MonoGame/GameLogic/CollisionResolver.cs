using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Resolves collisions between planets in the game.
/// </summary>
public class CollisionResolver
{
    private readonly GameStateHandler _gameStateHandler;
    private readonly PlanetMergingService _planetMergingService;

    /// <summary>
    ///     Constructor for the CollisionResolver class.
    ///     All dependencies are passed through this constructor and stored for use in the class methods.
    /// </summary>
    /// <param name="gameStateHandler">Provides the ability to check game state.</param>
    /// <param name="planetMergingService">Service to handle the merging behavior of two planets.</param>
    public CollisionResolver(GameStateHandler gameStateHandler, PlanetMergingService planetMergingService)
    {
        _gameStateHandler = gameStateHandler;
        _planetMergingService = planetMergingService;
    }

    /// <summary>
    ///     This method is the entry point to the collision handling logic.
    ///     It traverses all the passed pairs of potentially colliding planets and handles each collision.
    /// </summary>
    /// <param name="collisionPairs">List of pairs of potentially colliding planets.</param>
    public void HandleCollisions(List<(Planet, Planet)> collisionPairs)
    {
        foreach (var (planet1, planet2) in collisionPairs) HandleCollision(planet1, planet2);
    }

    /// <summary>
    ///     Handles the collision process between two planets.
    ///     If the planets are of the same type, they are merged into a new planet, otherwise, normal collision
    ///     processing is performed. It also checks if the game ended as a result of this collision.
    /// </summary>
    /// <param name="planet1">First planet in collision.</param>
    /// <param name="planet2">Second planet in collision.</param>
    private void HandleCollision(Planet planet1, Planet planet2)
    {
        // Merging planets 
        if (planet1.PlanetType == planet2.PlanetType)
        {
            // Handle same planet collision
            var mergedPlanet = _planetMergingService.MergePlanets(planet1, planet2);

            if (mergedPlanet == null) return; // No new planet to process

            _gameStateHandler.CheckWinCondition(mergedPlanet);
        }
        else
        {
            // Handle different planet collision
            HandleDifferentPlanetCollision(planet1, planet2);

            // Check if the game is over
            _gameStateHandler.CheckGameEndConditions(planet1);
            _gameStateHandler.CheckGameEndConditions(planet2);
        }
    }

    /// <summary>
    ///     Handles the collision between two different type of planets.
    ///     Corrects any overlap and simulates a bounce if the relative velocity is high enough.
    /// </summary>
    /// <param name="planet1">First planet in collision.</param>
    /// <param name="planet2">Second planet in collision.</param>
    private static void HandleDifferentPlanetCollision(Planet planet1, Planet planet2)
    {
        // 1. Overlap Correction
        CorrectOverlap(planet1, planet2);

        // 2. Simulate Bounce if velocity is high enough
        SimulateBounce(planet1, planet2);
    }

    /// <summary>
    ///     Corrects any overlap between two colliding planets.
    /// </summary>
    /// <param name="planet1">First planet in collision.</param>
    /// <param name="planet2">Second planet in collision.</param>
    private static void CorrectOverlap(Planet planet1, Planet planet2)
    {
        const float overlapCorrectionFactor = 0.5f;
        var overlap = planet1.Radius + planet2.Radius - Vector2.Distance(planet1.Position, planet2.Position);

        // Create a new vector normal and normalize it
        var normal = planet1.Position - planet2.Position;
        normal.Normalize();

        // Adjust the position of the planets
        var positionAdjustment = normal * overlap * overlapCorrectionFactor;
        var totalMass = planet1.Mass + planet2.Mass;
        var massRatio1 = planet2.Mass / totalMass;
        var massRatio2 = planet1.Mass / totalMass;

        planet1.Position += positionAdjustment * massRatio1;
        planet2.Position -= positionAdjustment * massRatio2;
    }

    /// <summary>
    ///     Simulates a bounce effect if the relative velocity of the colliding planets is high enough.
    /// </summary>
    /// <param name="planet1">First planet in collision.</param>
    /// <param name="planet2">Second planet in collision.</param>
    private static void SimulateBounce(VerletPoint planet1, VerletPoint planet2)
    {
        // Check if the velocity is high enough for a bounce
        const float velocityThreshold = 0.8f;
        var relativeVelocity = planet1.Velocity - planet2.Velocity;

        // Create a new vector normal and normalize it
        var normal = planet1.Position - planet2.Position;
        normal.Normalize();

        var velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);

        // If the velocity is not high enough, no bounce
        if (Math.Abs(velocityAlongNormal) < velocityThreshold) return;

        // Bounce the planets
        const float bounceFactor = 0.1f;
        var separationVelocity = normal * bounceFactor;
        planet1.Position += separationVelocity;
        planet2.Position -= separationVelocity;
    }
}