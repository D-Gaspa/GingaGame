using System;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame;

public class GameMode2Screen : GameScreen
{
    public GameMode2Screen(Game1 game) : base(game)
    {
    }

    public override void LoadContent()
    {
        // Load your content here
    }

    public override void Update(GameTime gameTime)
    {
        // Update your screen here
    }

    public override void Draw()
    {
        // Draw your screen here
    }

    public override void ResetGame()
    {
        throw new NotImplementedException();
    }

    public override void ResumeGame()
    {
        throw new NotImplementedException();
    }

    public override void PauseGame()
    {
        throw new NotImplementedException();
    }

    public Planet GetCurrentPlanet()
    {
        throw new NotImplementedException();
    }

    public void SetCurrentPlanet(Planet newPlanet)
    {
        throw new NotImplementedException();
    }
}