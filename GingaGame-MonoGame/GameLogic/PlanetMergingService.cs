using System;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Service responsible for handling the merging of two planets.
/// </summary>
public class PlanetMergingService
{
    private readonly GameMode _gameMode;
    private readonly GameMode2Screen _gameMode2Screen;
    private readonly PlanetFactory _planetFactory;
    private readonly Scene _scene;
    private readonly Score _score;

    /// <summary>
    ///     Initializes a new instance of the PlanetMergingService class.
    /// </summary>
    /// <param name="scene">The current scene.</param>
    /// <param name="gameMode">The current game mode.</param>
    /// <param name="planetFactory">The planet factory to create new planets.</param>
    /// <param name="score">The current score.</param>
    /// <param name="gameMode2Screen">The GameMode2Screen, if exists.</param>
    public PlanetMergingService(Scene scene, GameMode gameMode, PlanetFactory planetFactory, Score score,
        GameMode2Screen gameMode2Screen = null)
    {
        _scene = scene;
        _gameMode = gameMode;
        _gameMode2Screen = gameMode2Screen;
        _planetFactory = planetFactory;
        _score = score;
    }

    /// <summary>
    ///     Merges two planets taking into account the current game mode. The new planet is created
    ///     accordingly and added to the scene. The score is updated with the points of the new planet.
    /// </summary>
    /// <param name="planet1">The first planet to merge.</param>
    /// <param name="planet2">The second planet to merge.</param>
    /// <returns>The new planet resulting from the merge. Null if no new planet is created.</returns>
    public Planet MergePlanets(Planet planet1, Planet planet2)
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

        UpdateScoreWithPlanetPoints(mergedPlanet.Points);

        return mergedPlanet;
    }

    /// <summary>
    ///     Unlocks the next planet type according to the current game mode.
    /// </summary>
    /// <param name="planet1">The first planet in the merge.</param>
    /// <param name="planet2">The second planet in the merge.</param>
    /// <returns>A boolean indicating whether a new planet type was unlocked.</returns>
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

    /// <summary>
    ///     Creates a new planet resulting from the merge of two planets.
    /// </summary>
    /// <param name="planet1">The first planet in the merge.</param>
    /// <param name="planet2">The second planet in the merge.</param>
    /// <returns>The new planet.</returns>
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

    /// <summary>
    ///     Increases the player's score by the specified number of points.
    /// </summary>
    /// <param name="planetScore">The number of points to increase the score by.</param>
    private void UpdateScoreWithPlanetPoints(int planetScore)
    {
        _score.IncreaseScore(planetScore);
        _score.HasChanged = true;
    }
}