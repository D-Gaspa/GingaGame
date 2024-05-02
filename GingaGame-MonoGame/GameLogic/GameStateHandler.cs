﻿namespace GingaGame_MonoGame.GameLogic;

public class GameStateHandler
{
    private const int DrawEndLineThreshold = 70;
    private const int Tolerance = 5;
    private readonly Container _container;
    private readonly GameScreen _gameModeControl;
    private readonly Score _score;
    private readonly Scoreboard _scoreboard;
    private readonly UserInterfaceCreator _userInterfaceCreator;
    private bool _drawEndLine;
    private bool _gameOverTriggered;
    private bool _gameWonTriggered;

    public GameStateHandler(Container container, GameScreen gameModeControl, Score score,
        Scoreboard scoreboard, UserInterfaceCreator userInterfaceCreator)
    {
        _container = container;
        _gameModeControl = gameModeControl;
        _score = score;
        _scoreboard = scoreboard;
        _userInterfaceCreator = userInterfaceCreator;
    }

    public void Update()
    {
        if (!_drawEndLine)
            _container.HideEndLine();
        else
            _container.ShowEndLine();

        _drawEndLine = false;
    }

    public void CheckGameEndConditions(Planet planet)
    {
        if (!_gameOverTriggered) CheckLoseCondition(planet);

        if (IsNearEndLine(planet) && _drawEndLine == false) _drawEndLine = true;
    }

    private bool IsNearEndLine(Planet planet)
    {
        return planet.Position.Y < _container.TopLeft.Y + DrawEndLineThreshold + planet.Radius;
    }

    private void CheckLoseCondition(Planet planet)
    {
        if (!(planet.Position.Y < _container.TopLeft.Y + planet.Radius - Tolerance)) return;
        _gameOverTriggered = true;
        _userInterfaceCreator.ShowMessageWindow("Game Over! You lost!");

        // Check if the player won the game before losing
        if (_gameWonTriggered)
        {
            var (success, playerName) =
                _userInterfaceCreator.ShowInputDialog("Congratulations! You won!", "Enter your name:");
            if (success) _scoreboard.AddScore(playerName, _score.CurrentScore);
        }

        ResetGame();
    }

    public void CheckWinCondition(Planet planet)
    {
        if (planet.PlanetType != PlanetType.Sun || _gameWonTriggered) return;
        _gameWonTriggered = true;
        _userInterfaceCreator.ShowMessageWindow("Congratulations! You won!");
    }

    private void ResetGame()
    {
        _gameModeControl.ResetGame();
        _gameOverTriggered = false;
        _gameWonTriggered = false;
    }
}