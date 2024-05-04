using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame.GameLogic;

/// <summary>
///     Represents a point in a Verlet physics simulation.
/// </summary>
public class VerletPoint
{
    private const float Friction = 0.8f;
    private readonly Vector2 _gravity = new(0, 2.5f);
    public bool IsPinned;
    public Vector2 OldPosition;
    public Vector2 Position;
    public Vector2 Velocity;

    /// <summary>
    ///     Creates a new instance of the <see cref="VerletPoint"/> class.
    /// </summary>
    /// <param name="position">Initial position of the point</param>
    /// <param name="radius">Radius of the point used to calculate the mass</param>
    protected VerletPoint(Vector2 position, float radius)
    {
        Position = OldPosition = position;
        Mass = radius / 10;
    }

    /// <summary>
    ///     Returns the mass of the point.
    /// </summary>
    public float Mass { get; }

    /// <summary>
    ///     Updates the state of the point according to Verlet integration.
    /// </summary>
    public void Update()
    {
        if (IsPinned) return;
        UpdateVelocity();
        UpdatePosition();
    }

    /// <summary>
    ///     Updates the velocity of the VerletPoint based on Verlet integration.
    /// </summary>
    private void UpdateVelocity()
    {
        Velocity = Position - OldPosition;
        Velocity *= Friction;
    }

    /// <summary>
    ///     Updates the position of the VerletPoint based on Verlet integration.
    /// </summary>
    private void UpdatePosition()
    {
        // Save current position
        OldPosition = Position;

        // Perform Verlet integration
        Position += Velocity + _gravity;
    }
}