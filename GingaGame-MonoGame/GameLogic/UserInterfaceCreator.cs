using Myra.Graphics2D;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame.GameLogic;

public class UserInterfaceCreator
{
    private readonly Desktop _desktop;

    public UserInterfaceCreator(Desktop desktop)
    {
        _desktop = desktop;
    }

    public void ShowMessageWindow(string title)
    {
        var window = new Window
        {
            Title = title,
            Width = 200,
            Padding = new Thickness(8)
        };

        var okButton = new Button
        {
            Content = new Label { Text = "OK" },
            HorizontalAlignment = HorizontalAlignment.Center
        };

        okButton.Click += (_, _) => { window.Close(); };

        window.Content = okButton;

        window.ShowModal(_desktop);
    }

    public (bool, string) ShowInputDialog(string title, string message)
    {
        var success = false;

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
            if (dialog.Result) success = true;
        };

        dialog.ShowModal(_desktop);

        return (success, textBox1.Text);
    }
}