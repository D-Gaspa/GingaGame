namespace GingaGame_MonoGame.GameLogic;

public class Score
{
    public int CurrentScore { get; private set; }
    public bool HasChanged { get; set; }

    public void IncreaseScore(int amount)
    {
        CurrentScore += amount;
        HasChanged = true;
    }

    public void ResetScore()
    {
        HasChanged = true;
        CurrentScore = 0;
    }
}