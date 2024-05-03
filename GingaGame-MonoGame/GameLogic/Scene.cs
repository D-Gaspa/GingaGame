using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public class Scene
{
    public readonly Container Container;

    public Scene(Container container)
    {
        Container = container;
    }

    public List<Planet> Planets { get; } = new();
    public List<Floor> Floors { get; } = new();

    public void AddPlanet(Planet planet)
    {
        Planets.Add(planet);
    }

    public void RemovePlanet(Planet planet)
    {
        Planets.Remove(planet);
    }

    private void AddFloor(Floor floor)
    {
        Floors.Add(floor);
    }

    public void ClearPlanets()
    {
        Planets.Clear();
    }

    public void Update()
    {
        // Update the planets
        foreach (var planet in Planets)
            planet.Update();
    }

    public void InitializeFloors(int floorHeight, int verticalTopMargin, int totalFloors = 4,
        List<int> planetsPerFloor = null)
    {
        // If the planets per floor list is not provided, set it to the default values
        planetsPerFloor ??= new List<int> { 3, 3, 4, 1 };

        // Explanation for default values:
        // Floors are only used on the second game mode, where the game starts with the biggest planet.
        // The first floor has 3 planets; this means that the floor will hold 3 planets. In this case, the biggest 3 (10, 9, 8).
        // The 'nextPlanetIndex' is the index of the next planet that will be accepted on the next floor.
        // Since the next planet will now get accepted on the next floor, the next planet index is 7. (Highest index - 3 planets on the floor)
        // The next floor has 3 planets; again, this means that the floor will hold 3 planets. In this case, the next 3 biggest planets (7, 6, 5).
        // The logic continues until the last floor, which always needs to have only 1 planet, the smallest one.

        // Calculate the next planet index for each floor based on the number of planets per floor
        var highestPlanetIndex = PlanetSizes.Sizes.Count - 1;
        var nextPlanetIndex = highestPlanetIndex - planetsPerFloor[0];
        for (var i = 0; i < totalFloors; i++)
        {
            var floor = new Floor
            {
                StartPositionY = i * floorHeight + verticalTopMargin,
                EndPositionY = (i + 1) * floorHeight + verticalTopMargin,
                Index = i,
                NextPlanetIndex = nextPlanetIndex
            };
            AddFloor(floor);

            // Calculate the next planet index for the next floor or set it to -1 for the last floor
            if (nextPlanetIndex > 0 && i + 1 < totalFloors)
                nextPlanetIndex -= planetsPerFloor[i + 1];
            else
                nextPlanetIndex = -1;
        }
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

        // Check if the floor is within the visible range
        foreach (var floor in Floors.Where(floor =>
                     floor.EndPositionY >= visibleStartY && floor.StartPositionY <= visibleEndY))
            floor.Draw(spriteBatch, Container, yOffset);

        // Render the container if it's within the visible range
        if (Container.BottomLeft.Y >= visibleStartY && Container.TopLeft.Y <= visibleEndY)
            Container.Draw(spriteBatch, yOffset);
    }
}