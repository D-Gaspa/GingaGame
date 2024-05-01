using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
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
        var newScore = new ScoreEntry(FormatPlayerName(playerName), score);
        
        // Get the index of where the new score compares to lesser or equal scores
        var index = _scores.FindIndex(s => s.Score <= score);
        
        // Insert the new score at the correct position
        _scores.Insert(index >= 0 ? index : _scores.Count, newScore);

        AppendScoreToFile(newScore);
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

    private IEnumerable<ScoreEntry> GetScoreEntriesFromFile(string scoreFile)
    {
        using var storage = IsolatedStorageFile.GetUserStoreForApplication();
        if (!storage.FileExists(scoreFile))
            yield break;

        using var reader = new StreamReader(new IsolatedStorageFileStream(scoreFile, FileMode.Open, storage));

        var lines = reader.ReadToEnd().Split('\n');

        foreach (var line in lines)
        {
            var parts = line.Split(':');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var score))
                continue;

            yield return new ScoreEntry(FormatPlayerName(parts[0]), score);
        }
    }

    private void AppendScoreToFile(ScoreEntry scoreEntry)
    {
        using var storage = IsolatedStorageFile.GetUserStoreForApplication();
        using var writer = new StreamWriter(new IsolatedStorageFileStream(_scoreFile, FileMode.Append, storage));

        writer.WriteLine($"{scoreEntry.PlayerName}:{scoreEntry.Score}");
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