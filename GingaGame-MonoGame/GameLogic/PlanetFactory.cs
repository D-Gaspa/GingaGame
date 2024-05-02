using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     A factory class to generate new planets and manage unlocked types based on the game mode.
/// </summary>
public class PlanetFactory
{
    private readonly GameMode _gameMode;
    private readonly Random _randomGenerator = new();
    private List<PlanetType> _unlockedPlanets;

    /// <summary>
    ///     Constructor of the PlanetFactory class.
    /// </summary>
    /// <param name="gameMode">The current game mode (GameMode.Mode1 or GameMode.Mode2).</param>
    public PlanetFactory(GameMode gameMode)
    {
        _gameMode = gameMode;
        _unlockedPlanets = new List<PlanetType>();
        InitializeDefaultPlanetByGameMode();
    }

    /// <summary>
    ///     Generates and returns the next planet according to the game mode.
    /// </summary>
    /// <param name="displayWidth">Screen width.</param>
    /// <returns>The newly created planet.</returns>
    public Planet GenerateNextPlanet(float displayWidth)
    {
        PlanetType nextPlanetType;
        do
        {
            nextPlanetType = GetPlanetTypeByGameMode();
        } while (!_unlockedPlanets.Contains(nextPlanetType));

        var middleX = displayWidth / 2;
        return new Planet(nextPlanetType, new Vector2(middleX, 0), true);
    }

    /// <summary>
    ///     Unlocks the given type of planet, allowing it to be generated.
    /// </summary>
    /// <param name="planetType">The type of the planet to unlock.</param>
    public void UnlockPlanet(PlanetType planetType)
    {
        if (!_unlockedPlanets.Contains(planetType)) _unlockedPlanets.Add(planetType);
    }

    /// <summary>
    ///     Resets the unlocked planets to the default planet type based on the game mode.
    /// </summary>
    public void InitializeDefaultPlanetByGameMode()
    {
        _unlockedPlanets.Clear();

        _unlockedPlanets = _gameMode switch
        {
            GameMode.Mode1 => new List<PlanetType> { PlanetType.Pluto }, // Start with Pluto
            GameMode.Mode2 => new List<PlanetType> { PlanetType.Sun }, // Start with Sun
            _ => throw new ArgumentException("Invalid game mode")
        };
    }

    /// <summary>
    ///     Returns a type of planet according to the game mode.
    /// </summary>
    /// <returns>Returns a PlanetType based on the game mode.</returns>
    private PlanetType GetPlanetTypeByGameMode()
    {
        return _gameMode switch
        {
            GameMode.Mode1 => (PlanetType)_randomGenerator.Next(0, 5),
            GameMode.Mode2 => (PlanetType)_randomGenerator.Next(6, 11),
            _ => throw new ArgumentException("Invalid game mode")
        };
    }
}