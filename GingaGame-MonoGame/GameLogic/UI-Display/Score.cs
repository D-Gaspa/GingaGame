namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Represents the player's score in the game.
/// </summary>
public class Score
{
    /// <summary>
    ///     Gets the player's current score.
    /// </summary>
    public int CurrentScore { get; private set; }

    /// <summary>
    ///     Gets or sets a flag indicating whether the score has changed.
    /// </summary>
    public bool HasChanged { get; set; }

    /// <summary>
    ///     Increases the player's score by the specified amount.
    /// </summary>
    /// <param name="amount">The amount by which to increase the score.</param>
    public void IncreaseScore(int amount)
    {
        CurrentScore += amount;
        HasChanged = true;
    }

    /// <summary>
    ///     Resets the player's score.
    /// </summary>
    public void ResetScore()
    {
        HasChanged = true;
        CurrentScore = 0;
    }
}