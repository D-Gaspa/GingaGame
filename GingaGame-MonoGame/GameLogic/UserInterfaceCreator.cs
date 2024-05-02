using System;
using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame.GameLogic;

public class UserInterfaceCreator
{
    private readonly Desktop _desktop;
    private readonly GameScreen _gameScreen;

    public UserInterfaceCreator(Desktop desktop, GameScreen gameScreen)
    {
        _desktop = desktop;
        _gameScreen = gameScreen;
    }

    public void ShowMessageWindow(string title)
    {
        _gameScreen.PauseGame();

        var window = new Window
        {
            Title = title,
            Padding = new Thickness(8)
        };

        var okButton = new Button
        {
            Content = new Label { Text = "OK" },
            HorizontalAlignment = HorizontalAlignment.Center
        };

        okButton.Click += (_, _) =>
        {
            window.Close();
            _gameScreen.ResumeGame();
        };

        window.Content = okButton;

        window.ShowModal(_desktop);
    }

    public void ShowInputDialog(string title, string message, Action<bool, string> callback)
    {
        _gameScreen.PauseGame();

        var dialog = new Dialog { Title = title };

        var stackPanel = new HorizontalStackPanel { Spacing = 8 };

        var label1 = new Label { Text = message };

        stackPanel.Widgets.Add(label1);

        var textBox1 = new TextBox();
        StackPanel.SetProportionType(textBox1, ProportionType.Fill);
        stackPanel.Widgets.Add(textBox1);

        dialog.Content = stackPanel;

        dialog.Closed += (_, _) =>
        {
            callback(dialog.Result, textBox1.Text);
            _gameScreen.ResumeGame();
        };

        dialog.ShowModal(_desktop);
    }
}