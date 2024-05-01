using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;

namespace GingaGame_MonoGame.GameLogic;

public class Scoreboard
{
    private static string _scoreFile;
    private readonly List<ScoreEntry> _scores = new();

    public Scoreboard(GameMode gameMode)
    {
        _scoreFile = gameMode switch
        {
            GameMode.Mode1 => "scores1.txt",
            GameMode.Mode2 => "scores2.txt",
            _ => throw new ArgumentException("Invalid game mode")
        };
        LoadScores();
    }

    public void AddScore(string playerName, int score)
    {
        var newScore = new ScoreEntry(playerName, score);
        _scores.Add(newScore);
        _scores.Sort((x, y) => y.Score.CompareTo(x.Score)); // Sort descending

        // Append the new score to the file
        using var storage = IsolatedStorageFile.GetUserStoreForApplication();
        using var writer = new StreamWriter(new IsolatedStorageFileStream(_scoreFile, FileMode.Append, storage));
        writer.WriteLine($"{newScore.PlayerName}:{newScore.Score}");
    }

    public IEnumerable<ScoreEntry> GetTopScores()
    {
        _scores.Clear();
        LoadScores();
        return _scores;
    }

    private void LoadScores()
    {
        OrderScores();
        using var storage = IsolatedStorageFile.GetUserStoreForApplication();
        if (!storage.FileExists(_scoreFile)) return;
        using var reader = new StreamReader(new IsolatedStorageFileStream(_scoreFile, FileMode.Open, storage));
        
        var lines = reader.ReadToEnd().Split('\n');

        // Parse the lines and add the first 5 scores
        var count = 0;
        foreach (var line in lines)
        {
            if (count >= 5) break; // Only load the top 6 scores

            var parts = line.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var score)) continue;
            var playerName = parts[0];
            if (playerName.Length > 8) // Check if the name is too long
                playerName = playerName.Substring(0, 8) + ".."; // Trim and append '...'
            // Convert to title case
            playerName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(playerName.ToLowerInvariant());
            _scores.Add(new ScoreEntry(playerName, score));

            count++; // Increment the count
        }

        _scores.Sort((x, y) => y.Score.CompareTo(x.Score)); // Sort descending
    }

    private static void OrderScores()
    {
        using var storage = IsolatedStorageFile.GetUserStoreForApplication();
        if (!storage.FileExists(_scoreFile)) return;
        using var reader = new StreamReader(new IsolatedStorageFileStream(_scoreFile, FileMode.Open, storage));
        
        var lines = reader.ReadToEnd().Split('\n');
        // Parse the lines into a list of ScoreEntry objects
        var scoreEntries = new List<ScoreEntry>();
        foreach (var line in lines)
        {
            var parts = line.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var score)) continue;
            var playerName = parts[0];
            scoreEntries.Add(new ScoreEntry(playerName, score));
        }

        // Sort the list in descending order by score
        scoreEntries.Sort((x, y) => y.Score.CompareTo(x.Score));

        // Write the sorted scores back to the file
        using var writer = new StreamWriter(_scoreFile);
        foreach (var entry in scoreEntries) writer.WriteLine($"{entry.PlayerName}:{entry.Score}");
    }

    public class ScoreEntry
    {
        public ScoreEntry(string playerName, int score)
        {
            PlayerName = playerName;
            Score = score;
        }

        public string PlayerName { get; }
        public int Score { get; }
    }
}