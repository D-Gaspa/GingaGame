using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame.GameLogic;

public class GameStateHandler
{
    private const int DrawEndLineThreshold = 70;
    private const int Tolerance = 5;
    private readonly Container _container;
    private readonly Desktop _desktop;
    private readonly GameScreen _gameModeControl;
    private readonly Score _score;
    private readonly Scoreboard _scoreboard;
    private bool _drawEndLine;
    private bool _gameOverTriggered;
    private bool _gameWonTriggered;

    public GameStateHandler(Container container, Desktop desktop, GameScreen gameModeControl, Score score,
        Scoreboard scoreboard)
    {
        _container = container;
        _desktop = desktop;
        _gameModeControl = gameModeControl;
        _score = score;
        _scoreboard = scoreboard;
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
        ShowMessageWindow("Game Over! You lost!");

        // Check if the player won the game before losing
        if (_gameWonTriggered)
        {
            var (success, playerName) = ShowInputDialog("Congratulations! You won!", "Enter your name:");
            if (success) _scoreboard.AddScore(playerName, _score.CurrentScore);
        }

        ResetGame();
    }

    public void CheckWinCondition(Planet planet)
    {
        if (planet.PlanetType != PlanetType.Sun || _gameWonTriggered) return;
        _gameWonTriggered = true;
        ShowMessageWindow("Congratulations! You won!");
    }

    private void ResetGame()
    {
        _gameModeControl.ResetGame();
        _gameOverTriggered = false;
        _gameWonTriggered = false;
    }

    private void ShowMessageWindow(string title)
    {
        var window = new Window
        {
            Title = title,
            Width = 200,
            Padding = new Thickness(8)
        };

        var okButton = new Button
        {
            Content = new Label
            {
                Text = "OK"
            },
            HorizontalAlignment = HorizontalAlignment.Center
        };

        okButton.Click += (_, _) => { window.Close(); };

        window.Content = okButton;

        window.ShowModal(_desktop);
    }

    private (bool, string) ShowInputDialog(string title, string message)
    {
        var success = false;

        var dialog = new Dialog
        {
            Title = title
        };

        var stackPanel = new HorizontalStackPanel
        {
            Spacing = 8
        };

        var label1 = new Label
        {
            Text = message
        };
        stackPanel.Widgets.Add(label1);

        var textBox1 = new TextBox();
        StackPanel.SetProportionType(textBox1, ProportionType.Fill);
        stackPanel.Widgets.Add(textBox1);

        dialog.Content = stackPanel;

        dialog.Closed += (_, _) =>
        {
            if (dialog.Result) success = true;
        };

        dialog.ShowModal(_desktop);

        return (success, textBox1.Text);
    }
}