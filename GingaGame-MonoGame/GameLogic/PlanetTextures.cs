using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GingaGame_MonoGame.GameLogic;

public static class PlanetTextures
{
    private static readonly Dictionary<PlanetType, Texture2D> CachedTextures = new();
    private static ContentManager _contentManager;

    public static void SetContentManager(ContentManager manager)
    {
        _contentManager = manager;
    }

    public static Texture2D GetCachedTexture(PlanetType planetType)
    {
        if (CachedTextures.TryGetValue(planetType, out var texture)) return texture;

        // Load and cache the texture
        var textureName = GetTextureName(planetType);

        CachedTextures[planetType] = _contentManager.Load<Texture2D>("Resources/" + textureName);

        return CachedTextures[planetType];
    }

    private static string GetTextureName(PlanetType planetType)
    {
        return planetType.ToString();
    }
}