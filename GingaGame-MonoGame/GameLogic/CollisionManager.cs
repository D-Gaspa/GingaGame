namespace GingaGame_MonoGame.GameLogic;

public class CollisionManager
{
    private readonly CollisionDetector _detector;
    private readonly CollisionResolver _resolver;

    public CollisionManager(Container container, GameMode gameMode, GameStateHandler gameStateHandler,
        PlanetFactory planetFactory, Scene scene, Score score, GameMode2Screen gameMode2Screen)
    {
        _detector = new CollisionDetector(scene.Planets);
        _resolver = new CollisionResolver(container, gameMode, gameStateHandler, planetFactory, scene, score,
            gameMode2Screen);
    }

    public void RunCollisions(int iterations)
    {
        for (var i = 0; i < iterations; i++) RunCollisions();
    }

    private void RunCollisions()
    {
        bool needsNewCollisionCheck;

        do
        {
            var potentialCollisions = _detector.CheckCollisions();
            needsNewCollisionCheck =
                _resolver.HandleCollisions(potentialCollisions); // returns if a new detection is required
        } while (needsNewCollisionCheck);

        _resolver.NeedsNewCollisionCheck = false; // Reset the flag for the next update
    }
}