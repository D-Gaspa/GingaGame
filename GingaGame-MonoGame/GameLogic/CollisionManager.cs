namespace GingaGame_MonoGame.GameLogic;

public class CollisionManager
{
    private readonly CollisionDetector _detector;
    private readonly CollisionResolver _resolver;

    public CollisionManager(Scene scene, PlanetFactory planetFactory, Score score, Container container,
        GameMode gameMode, GameMode2Screen gameMode2Screen = null)
    {
        _detector = new CollisionDetector(scene.Planets);
        _resolver = new CollisionResolver(scene, planetFactory, score, container, gameMode, gameMode2Screen);
    }

    public void RunCollisions()
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