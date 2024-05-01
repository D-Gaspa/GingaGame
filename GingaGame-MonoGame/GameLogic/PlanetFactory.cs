using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame.GameLogic;

public class PlanetFactory
{
    private readonly GameMode _gameMode;
    private readonly Random _randomGenerator = new();
    private List<PlanetType> _unlockedPlanets;

    public PlanetFactory(GameMode gameMode)
    {
        _gameMode = gameMode;
        _unlockedPlanets = new List<PlanetType>();
        InitializeDefaultPlanetByGameMode();
    }

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
    
    public void UnlockPlanet(PlanetType planetType)
    {
        if (!_unlockedPlanets.Contains(planetType)) _unlockedPlanets.Add(planetType);
    }
    
    private void InitializeDefaultPlanetByGameMode()
    {
        _unlockedPlanets.Clear();
        
        _unlockedPlanets = _gameMode switch
        {
            GameMode.Mode1 => new List<PlanetType> { PlanetType.Earth }, // Start with Earth
            GameMode.Mode2 => new List<PlanetType> { PlanetType.Sun }, // Start with Sun
            _ => throw new ArgumentException("Invalid game mode")
        };
    }
    
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