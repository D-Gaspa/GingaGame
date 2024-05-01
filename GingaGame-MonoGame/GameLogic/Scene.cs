using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public class Scene
{
    private List<Planet> Planets { get; } = new();
    private List<Floor> Floors { get; } = new();

    public void AddPlanet(Planet planet)
    {
        Planets.Add(planet);
    }

    public void RemovePlanet(Planet planet)
    {
        Planets.Remove(planet);
    }

    public void AddFloor(Floor floor)
    {
        Floors.Add(floor);
    }

    public void ClearPlanets()
    {
        Planets.Clear();
    }

    public void Draw(SpriteBatch spriteBatch, float displayHeight, float yOffset = 0)
    {
        // Calculate the visible range
        var visibleStartY = yOffset;
        var visibleEndY = yOffset + displayHeight;

        // Check if the planets are within the visible range
        foreach (var planet in Planets.Where(planet =>
                     planet.Position.Y + planet.Radius >= visibleStartY &&
                     planet.Position.Y - planet.Radius <= visibleEndY))
            planet.Draw(spriteBatch, yOffset);

        // TODO: Add drawing for floors
    }
}