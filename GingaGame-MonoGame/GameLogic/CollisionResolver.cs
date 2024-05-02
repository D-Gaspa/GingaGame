using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame.GameLogic;

public class CollisionResolver
{
    private readonly Container _container;
    private readonly GameMode _gameMode;
    private readonly GameMode2Screen _gameMode2Screen;
    private readonly GameStateHandler _gameStateHandler;
    private readonly PlanetFactory _planetFactory;
    private readonly Scene _scene;
    private readonly Score _score;

    public bool NeedsNewCollisionCheck;

    public CollisionResolver(Container container, GameMode gameMode, GameStateHandler gameStateHandler,
        PlanetFactory planetFactory, Scene scene, Score score,
        GameMode2Screen gameMode2Screen = null)
    {
        _container = container;
        _gameMode = gameMode;
        _gameStateHandler = gameStateHandler;
        _planetFactory = planetFactory;
        _scene = scene;
        _score = score;
        _gameMode2Screen = gameMode2Screen;
    }

    public bool HandleCollisions(List<(Planet, Planet)> collisionPairs)
    {
        foreach (var (planet1, planet2) in collisionPairs) HandleCollision(planet1, planet2);
        return NeedsNewCollisionCheck;
    }

    private void HandleCollision(Planet planet1, Planet planet2)
    {
        // Merging planets 
        if (planet1.PlanetType == planet2.PlanetType)
        {
            // Handle same planet collision
            var mergedPlanet = MergePlanets(planet1, planet2);

            if (mergedPlanet == null) return; // No new planet to process

            _gameStateHandler.CheckWinCondition(mergedPlanet);

            // Set the flag to check for new collisions as the new planet might collide with others
            NeedsNewCollisionCheck = true;
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

    private Planet MergePlanets(Planet planet1, Planet planet2)
    {
        _scene.RemovePlanet(planet1);
        _scene.RemovePlanet(planet2);

        // Unlock new planet (if needed)
        if (!UnlockNextPlanetType(planet1, planet2)) return null;

        // Create a new planet
        var mergedPlanet = CreateMergedPlanet(planet1, planet2);

        // Update the current planet in GameMode2 if needed
        if (_gameMode == GameMode.Mode2)
        {
            var currentPlanet = _gameMode2Screen.GetCurrentPlanet();
            if (currentPlanet == planet1 || currentPlanet == planet2)
                _gameMode2Screen.SetCurrentPlanet(mergedPlanet);
        }

        // Add the new planet to the scene
        _scene.AddPlanet(mergedPlanet);

        // Update scores for game mode 1
        if (_gameMode == GameMode.Mode1)
            UpdateScoreWithPlanetPoints(mergedPlanet.Points);

        return mergedPlanet;
    }

    private bool UnlockNextPlanetType(Planet planet1, Planet planet2)
    {
        switch (_gameMode)
        {
            case GameMode.Mode1:
                if ((int)planet1.PlanetType + 1 >= 11) // if the largest planet is reached
                {
                    const int largestPlanetScore = 100;
                    UpdateScoreWithPlanetPoints(largestPlanetScore);
                    return false; // No new planet to unlock
                }

                // Unlock the next planet
                _planetFactory.UnlockPlanet(planet1.PlanetType + 1);
                break;

            case GameMode.Mode2:
                if ((int)planet2.PlanetType - 1 <= 0) // if the smallest planet is reached
                    return false; // No new planet to unlock

                // Unlock the previous planet
                _planetFactory.UnlockPlanet(planet2.PlanetType - 1);
                break;
            default:
                throw new ArgumentException("Invalid game mode");
        }

        return true;
    }

    private Planet CreateMergedPlanet(Planet planet1, Planet planet2)
    {
        // The position of the new planet will be the middle point between the two planets
        var middlePoint = (planet1.Position + planet2.Position) / 2;

        var newPlanet = _gameMode switch
        {
            GameMode.Mode1 => new Planet(planet1.PlanetType + 1, middlePoint),
            GameMode.Mode2 => new Planet(planet2.PlanetType - 1, middlePoint),
            _ => throw new ArgumentException("Invalid game mode")
        };
        return newPlanet;
    }

    private void UpdateScoreWithPlanetPoints(int largestPlanetScore)
    {
        _score.IncreaseScore(largestPlanetScore);
        _score.HasChanged = true;
    }

    private static void HandleDifferentPlanetCollision(Planet planet1, Planet planet2)
    {
        // 1. Overlap Correction
        CorrectOverlap(planet1, planet2);

        // 2. Simulate Bounce if velocity is high enough
        SimulateBounce(planet1, planet2);
    }

    private static void CorrectOverlap(Planet planet1, Planet planet2)
    {
        const float overlapCorrectionFactor = 0.5f;
        var overlap = planet1.Radius + planet2.Radius - Vector2.Distance(planet1.Position, planet2.Position);

        // Create a new vector normal and normalize it
        var normal = planet1.Position - planet2.Position;
        normal.Normalize();

        var positionAdjustment = normal * overlap * overlapCorrectionFactor;
        var totalMass = planet1.Mass + planet2.Mass;
        var massRatio1 = planet2.Mass / totalMass;
        var massRatio2 = planet1.Mass / totalMass;

        planet1.Position += positionAdjustment * massRatio1;
        planet2.Position -= positionAdjustment * massRatio2;
    }

    private static void SimulateBounce(Planet planet1, Planet planet2)
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

        planet1.HasCollided = true;
        planet2.HasCollided = true;
    }

    public void CheckConstraints(Planet planet)
    {
        WallConstraints(planet);
        ContainerConstraints(planet);
        if (_gameMode == GameMode.Mode2) FloorConstraints(planet);
    }

    private static void WallConstraints(Planet planet)
    {
        // Check if the point is outside the top boundary of the wall
        if (planet.Position.Y < planet.Radius) planet.Position.Y = planet.Radius;
    }

    private void ContainerConstraints(Planet planet)
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

    private void FloorConstraints(Planet planet)
    {
        // TODO: Implement FloorConstraints
        // if (_gameMode != GameMode.Mode2) return; // Apply only in GameMode2
        //
        // // Find the current floor
        // var floor = _scene.Floors.FirstOrDefault(f =>
        //     f.StartPositionY <= planet.Position.Y && planet.Position.Y <= f.EndPositionY);
        //
        // if (floor == null) return; // Planet is outside the floor range
        //
        // // Check if the planet can pass through the floor
        // if (planet.PlanetType <= floor.NextPlanetIndex)
        //     // Can pass - no collision
        //     return;
        //
        // const int floorEndPositionHeight = 30;
        //
        // // Handle Collision (similar to container boundaries)
        // if (!(planet.Position.Y > floor.EndPositionY - floorEndPositionHeight - planet.Radius)) return;
        //
        // if (floor.NextPlanetIndex == -1) // Last floor
        //     // Game Won
        //     gameMode2Control.GameWon();
        // planet.Position.Y = floor.EndPositionY - floorEndPositionHeight - planet.Radius;
    }

    public bool IsGameOver()
    {
        // Check if a planet has passed the end line
        return _scene.Planets.Any(planet =>
            planet.Position.Y < _container.TopLeft.Y + planet.Radius && planet.HasCollided);
    }
}