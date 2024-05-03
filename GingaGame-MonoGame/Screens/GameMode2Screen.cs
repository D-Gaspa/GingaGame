using System;
using GingaGame_MonoGame.GameLogic;
using Microsoft.Xna.Framework;
using Myra.Graphics2D.UI;

namespace GingaGame_MonoGame;

public class GameMode2Screen : GameModeScreenBase
{
    protected override GameMode Mode => GameMode.Mode1;

    public GameMode2Screen(Game1 game, Desktop desktop) : base(game, desktop)
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

    public Planet GetCurrentPlanet()
    {
        throw new NotImplementedException();
    }

    public void SetCurrentPlanet(Planet newPlanet)
    {
        throw new NotImplementedException();
    }
}