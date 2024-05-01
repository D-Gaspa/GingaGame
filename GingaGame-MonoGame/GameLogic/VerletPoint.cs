using Microsoft.Xna.Framework;

namespace GingaGame_MonoGame.GameLogic;

public class VerletPoint
{
    private const float Friction = 0.8f;
    private readonly Vector2 _gravity = new(0, 0.9f);
    public bool IsPinned;
    public Vector2 OldPosition;
    public Vector2 Position;
    public Vector2 Velocity;

    protected VerletPoint(Vector2 position, float radius)
    {
        Position = OldPosition = position;
        Mass = radius / 10;
    }

    public float Mass { get; }

    public void Update()
    {
        if (IsPinned) return;
        UpdateVelocity();
        UpdatePosition();
    }

    private void UpdateVelocity()
    {
        Velocity = Position - OldPosition;
        Velocity *= Friction;
    }

    private void UpdatePosition()
    {
        // Save current position
        OldPosition = Position;

        // Perform Verlet integration
        Position += Velocity + _gravity;
    }
}