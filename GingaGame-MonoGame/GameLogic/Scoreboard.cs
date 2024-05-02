using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GingaGame_MonoGame.GameLogic;

public class Scoreboard
{
    private readonly string _scoreFile;
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

        // Load all scores from the file
        var allScores = GetScoreEntriesFromFile(_scoreFile).ToList();

        // Add the new score
        allScores.Add(newScore);

        // Sort all scores in descending order
        allScores.Sort((x, y) => y.Score.CompareTo(x.Score));

        // Rewrite the entire file with the sorted scores
        using var writer = new StreamWriter(_scoreFile);
        foreach (var entry in allScores) writer.WriteLine($"{entry.PlayerName}:{entry.Score}");
    }

    public IEnumerable<ScoreEntry> GetTopScores()
    {
        _scores.Clear();

        LoadScores();

        return _scores;
    }

    private void LoadScores()
    {
        var scoreEntries = GetScoreEntriesFromFile(_scoreFile).ToList();

        scoreEntries.Sort((x, y) => y.Score.CompareTo(x.Score));

        _scores.AddRange(scoreEntries.Take(5));
    }

    private static IEnumerable<ScoreEntry> GetScoreEntriesFromFile(string scoreFile)
    {
        if (!File.Exists(scoreFile))
            yield break;

        using var reader = new StreamReader(scoreFile);

        var lines = reader.ReadToEnd().Split('\n');

        foreach (var line in lines)
        {
            var parts = line.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var score))
                continue;

            yield return new ScoreEntry(FormatPlayerName(parts[0]), score);
        }
    }

    private static string FormatPlayerName(string playerName)
    {
        if (playerName.Length > 8)
            playerName = playerName[..8] + "..";

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(playerName.ToLowerInvariant());
    }
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