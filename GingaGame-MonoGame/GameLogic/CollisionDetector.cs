using System.Collections.Generic;
using System.Drawing;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     The CollisionDetector class is used to check if collisions are happening between any of the Planets.
/// </summary>
public class CollisionDetector
{
    private readonly List<Planet> _planets;
    private readonly List<(Planet, Planet)> _potentialCollisionPairs = new();

    /// <summary>
    ///     Initializes a new instance of the CollisionDetector class.
    /// </summary>
    /// <param name="planets">List of the planets in the current game scene.</param>
    public CollisionDetector(List<Planet> planets)
    {
        _planets = planets;
    }

    /// <summary>
    ///     Checks for collisions and return a list of pairs of planets that are colliding.
    /// </summary>
    /// <returns>
    ///     Returns a list tuples, where each tuple contains two planets that are colliding.
    /// </returns>
    public List<(Planet, Planet)> CheckCollisions()
    {
        _potentialCollisionPairs.Clear();
        BroadPhaseCheck();
        return NarrowPhaseCheck();
    }

    /// <summary>
    ///     Implements the Broad Phase of the collision detection using Axis-Aligned Bounding Boxes (AABB).
    /// </summary>
    private void BroadPhaseCheck()
    {
        for (var i = 0; i < _planets.Count; i++)
        for (var j = i + 1; j < _planets.Count; j++)
        {
            var planet1 = _planets[i];
            var planet2 = _planets[j];

            // If the planets are pinned, they cannot collide
            if (planet1.IsPinned || planet2.IsPinned) continue;

            // Calculate bounding boxes
            if (!DoBoundingBoxesIntersect(planet1, planet2)) continue; // No potential collision

            // Potential collision - pass to Narrow Phase
            _potentialCollisionPairs.Add((planet1, planet2));
        }
    }

    /// <summary>
    ///     Checks if two planets bounding boxes intersect with each other.
    /// </summary>
    /// <param name="planet1">The first planet.</param>
    /// <param name="planet2">The second planet.</param>
    /// <returns>Returns true if boxes intersect, false otherwise.</returns>
    private static bool DoBoundingBoxesIntersect(Planet planet1, Planet planet2)
    {
        var box1 = new RectangleF(planet1.Position.X - planet1.Radius, planet1.Position.Y - planet1.Radius,
            planet1.Radius * 2, planet1.Radius * 2);
        var box2 = new RectangleF(planet2.Position.X - planet2.Radius, planet2.Position.Y - planet2.Radius,
            planet2.Radius * 2, planet2.Radius * 2);

        return box1.IntersectsWith(box2);
    }

    /// <summary>
    ///     Implements the Narrow Phase of the collision detection by checking the real shapes.
    /// </summary>
    /// <returns>
    ///     Returns a list tuples, where each tuple contains two planets that are colliding.
    /// </returns>
    private List<(Planet, Planet)> NarrowPhaseCheck()
    {
        // Initialize list to store pairs of planets that are colliding
        var collisionPairs = new List<(Planet, Planet)>();

        // If there are no potential collision pairs, no need to check further
        if (_potentialCollisionPairs.Count == 0) return collisionPairs;

        // Create a copy of the list
        var potentialCollisionPairsCopy = new List<(Planet, Planet)>(_potentialCollisionPairs);

        // Iterate through pairs of planets that passed the broad phase
        foreach (var (planet1, planet2) in potentialCollisionPairsCopy)
        {
            // Recalculate distance (more accurate, as in broad-phase it might be an overestimate)
            var distanceX = planet1.Position.X - planet2.Position.X;
            var distanceY = planet1.Position.Y - planet2.Position.Y;
            var distanceSquared = distanceX * distanceX + distanceY * distanceY;
            var sumOfRadiiSquared = (planet1.Radius + planet2.Radius) * (planet1.Radius + planet2.Radius);

            if (distanceSquared <= sumOfRadiiSquared) // Collision detected
                collisionPairs.Add((planet1, planet2));
        }

        return collisionPairs;
    }
}