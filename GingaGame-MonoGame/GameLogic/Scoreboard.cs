using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Represents a scoreboard for the game, managing high scores and interfacing with storage.
/// </summary>
public class Scoreboard
{
    private readonly string _scoreFile;
    private readonly List<ScoreEntry> _scores = new();

    /// <summary>
    ///     Initializes a new instance of the Scoreboard class.
    /// </summary>
    /// <param name="gameMode">The game mode for which to manage scores.</param>
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

    /// <summary>
    ///     Adds a new entry to the scoreboard, persisting it to the appropriate storage.
    /// </summary>
    /// <param name="playerName">Player name to add to the scoreboard.</param>
    /// <param name="score">The score earned by the player.</param>
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

    /// <summary>
    ///     Returns the top scores from the scoreboard.
    /// </summary>
    /// <returns>The top scores in descending order.</returns>
    public IEnumerable<ScoreEntry> GetTopScores()
    {
        _scores.Clear();

        LoadScores();

        return _scores;
    }

    /// <summary>
    ///     Loads the scores from the storage file.
    /// </summary>
    private void LoadScores()
    {
        var scoreEntries = GetScoreEntriesFromFile(_scoreFile).ToList();

        scoreEntries.Sort((x, y) => y.Score.CompareTo(x.Score));

        _scores.AddRange(scoreEntries.Take(5));
    }

    /// <summary>
    ///     Reads the score entries from the specified file.
    /// </summary>
    /// <param name="scoreFile">The file to read the score entries from.</param>
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

    /// <summary>
    ///     Formats the player name for display in the scoreboard.
    /// </summary>
    /// <param name="playerName">The player name to format.</param>
    /// <returns></returns>
    private static string FormatPlayerName(string playerName)
    {
        if (playerName.Length > 8)
            playerName = playerName[..8] + "..";

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(playerName.ToLowerInvariant());
    }
}

/// <summary>
///     Represents an entry in a scoreboard, combining a player name and a score.
/// </summary>
public class ScoreEntry
{
    /// <summary>
    ///     Initializes a new instance of the ScoreEntry class.
    /// </summary>
    /// <param name="playerName">The player's name.</param>
    /// <param name="score">The player's score.</param>
    public ScoreEntry(string playerName, int score)
    {
        PlayerName = playerName;
        Score = score;
    }

    /// <summary>
    ///     Gets the player's name for the score entry.
    /// </summary>
    public string PlayerName { get; }

    /// <summary>
    ///     Gets the score value for the score entry.
    /// </summary>
    public int Score { get; }
}